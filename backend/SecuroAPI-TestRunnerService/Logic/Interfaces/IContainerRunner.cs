namespace SecuroAPI_TestRunnerService.Logic.Interfaces;

public interface IContainerRunner
{
    Task<(int ExitCode, string Stdout)> RunAsync(string targetUrl, IReadOnlyList<string> jobEnabledTestCheckIds,
        CancellationToken ct);
}