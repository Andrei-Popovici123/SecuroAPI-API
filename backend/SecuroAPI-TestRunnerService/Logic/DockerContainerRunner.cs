using Docker.DotNet;
using Docker.DotNet.Models;
using SecuroAPI_TestRunnerService.Logic.Interfaces;

namespace SecuroAPI_TestRunnerService.Logic;

public class DockerContainerRunner : IContainerRunner
{
    private const string Image = "securoapi-zap-runner:latest";
    private readonly ILogger<DockerContainerRunner> _logger;

    private readonly DockerClient _dockerClient = new DockerClientBuilder()
        .Build();

    public DockerContainerRunner(ILogger<DockerContainerRunner> logger)
    {
        _logger = logger;
    }


    public async Task<(int ExitCode, string Stdout)> RunAsync(string targetUrl, CancellationToken ct)
    {
        var create = await _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
        {
            Image = Image,
            Env = new List<string> { $"TARGET_URL={targetUrl}" },

            HostConfig = new HostConfig
            {
                Memory = 256L * 1024 * 1024,
                NanoCPUs = 500_000_000,
                PidsLimit = 64,
                AutoRemove = false
            }
        }, ct);

        var id = create.ID;

        try
        {
            await _dockerClient.Containers.StartContainerAsync(id, null, ct);

            var wait = await _dockerClient.Containers.WaitContainerAsync(id, ct);

            using var logs = await _dockerClient.Containers.GetContainerLogsAsync(id,
                new ContainerLogsParameters { ShowStdout = true, ShowStderr = true, Follow = false },
                CancellationToken.None);

            var (stdout, stderr) = await logs.ReadOutputToEndAsync(CancellationToken.None);

            if (!string.IsNullOrWhiteSpace(stderr))
                _logger.LogDebug("Container {Id} stderr: {Stderr}", id[..12], stderr);


            return ((int)wait.StatusCode, stdout);
        }
        catch (OperationCanceledException) 
        {
            await KillJobAsync(id);
            throw;
        }
        finally
        {
            await RemoveJobAsync(id);
        }
    }

    private async Task KillJobAsync(string id)
    {
        try
        {
            await _dockerClient.Containers.KillContainerAsync(id, new ContainerKillParameters(),
                CancellationToken.None);

            _logger.LogWarning("Killed container {Id} (scan cancelled or timed out)", id[..12]);
        }
        catch (DockerApiException ex)
        {
            _logger.LogDebug(ex, "Unable to kill job for {Id}", id[..12]);
        }
    }

    private async Task RemoveJobAsync(string id)
    {
        try
        {
            await _dockerClient.Containers.RemoveContainerAsync(
                id, new ContainerRemoveParameters { Force = true }, CancellationToken.None);
        }
        catch (DockerApiException ex)
        {
            _logger.LogDebug(ex, "Failed to remove container {Id} — leaked", id[..12]);
        }
    }
}