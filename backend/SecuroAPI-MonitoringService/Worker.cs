using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SecuroAPI_MonitoringService.Messaging;
using SecuroAPI.Contracts.Events;

namespace SecuroAPI_MonitoringService;

public class Worker(
    ILogger<Worker> logger,
    MonitoringResultPublisher monitoringResultPublisher,
    MonitoringRegisterConsumer monitoringRegisterConsumer) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await monitoringRegisterConsumer.StartJobAsync(stoppingToken);
        
        await monitoringResultPublisher.PublishAsync(
            new MonitoringMessage("Record Scratch"),
            stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}