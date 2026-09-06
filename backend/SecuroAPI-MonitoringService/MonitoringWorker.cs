using System.Text;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SecuroAPI_MonitoringService.Messaging;
using SecuroAPI.BusinessLogic.Services.Interfaces;
using SecuroAPI.Common.Constants;
using SecuroAPI.Contracts.Events;

namespace SecuroAPI_MonitoringService;

public class MonitoringWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly MonitoringOptions _options;
    private readonly ILogger<MonitoringWorker> _logger;

    public MonitoringWorker(
        IServiceProvider serviceProvider,
        IOptions<MonitoringOptions> options,
        ILogger<MonitoringWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Monitoring worker started: tick {Tick}s, batch {Batch}, concurrency {Concurrency}",
            _options.TickSeconds, _options.BatchSize, _options.Concurrency);

        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(_options.TickSeconds));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            using var scope = _serviceProvider.CreateScope();

            try
            {
                var runner = scope.ServiceProvider.GetRequiredService<IMonitoringRunnerService>();
                await runner.RunDueAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Monitoring tick failed");
            }
        }

        _logger.LogInformation("Monitoring worker stopped");
    }
}