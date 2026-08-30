using SecuroAPI.BusinessLogic.Services.Interfaces;

namespace SecuroAPI_API.Messaging;

public class StuckJobClearer : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(2);
    private static readonly TimeSpan StuckAfter = TimeSpan.FromMinutes(10);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<StuckJobClearer> _logger;

    public StuckJobClearer(IServiceScopeFactory scopeFactory, ILogger<StuckJobClearer> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var timer = new PeriodicTimer(Interval);

        while (await timer.WaitForNextTickAsync(cancellationToken))
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var jobService = scope.ServiceProvider.GetRequiredService<ITestJobService>();

                var result = await jobService.ClearStuckJobsAsync(StuckAfter);

                if (result.IsSuccess && result.Value > 0)
                    _logger.LogInformation("Cleared {Count} stuck Jobs", result.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to clear Stuck Jobs");
            }
        }
    }
}