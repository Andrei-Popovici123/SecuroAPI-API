namespace SecuroAPI.BusinessLogic.Services.Interfaces;

public interface IMonitoringRunnerService
{
    Task RunDueAsync(CancellationToken ct);
}