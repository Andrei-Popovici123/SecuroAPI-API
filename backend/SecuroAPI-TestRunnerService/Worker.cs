using System.Text;
using RabbitMQ.Client;
using SecuroAPI_TestRunnerService.Messaging;
using SecuroAPI.Contracts.Events;

namespace SecuroAPI_TestRunnerService;

public class Worker(ILogger<Worker> logger, TestResultPublisher testResultPublisher, TestJobConsumer consumer) : BackgroundService
{
 

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    { 
        

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