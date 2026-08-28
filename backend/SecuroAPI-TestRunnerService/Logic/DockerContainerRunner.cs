using Docker.DotNet;
using Docker.DotNet.Models;
using SecuroAPI_TestRunnerService.Logic.Interfaces;

namespace SecuroAPI_TestRunnerService.Logic;

public class DockerContainerRunner : IContainerRunner
{
    private const string Image = "securoapi-zap-runner:latest";

    private readonly DockerClient _dockerClient = new DockerClientBuilder()
        .Build();


    public async Task<(int ExitCode, string Stdout)> RunAsync(string targetUrl, CancellationToken ct)
    {
        var create = await _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
        {
            Image = Image,
            Env = new List<string> { $"TARGET_URL={targetUrl}" },

            HostConfig = new HostConfig
            {
                Memory     = 256L * 1024 * 1024,
                NanoCPUs   = 500_000_000,
                PidsLimit  = 64,
                AutoRemove = false
            }
        }, ct);

        await _dockerClient.Containers.StartContainerAsync(create.ID, null, ct);

        var wait = await _dockerClient.Containers.WaitContainerAsync(create.ID, ct);
        
        using var logs = await _dockerClient.Containers.GetContainerLogsAsync(
            create.ID,
            new ContainerLogsParameters { ShowStdout = true, ShowStderr = true, Follow = false },
            ct);

        var (stdout, _) = await logs.ReadOutputToEndAsync(ct);
        
        await _dockerClient.Containers.RemoveContainerAsync(
            create.ID, new ContainerRemoveParameters { Force = true }, ct);

        return ((int)wait.StatusCode, stdout);
    }
}