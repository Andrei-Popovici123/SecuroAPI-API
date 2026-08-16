using System.Text;
using RabbitMQ.Client;
using SecuroAPI_TestRunnerService.Messaging;
using SecuroAPI.Contracts.Events;

namespace SecuroAPI_TestRunnerService;

public class Worker(ILogger<Worker> logger, TestResultPublisher testResultPublisher, TestJobConsumer consumer) : BackgroundService
{
 

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await consumer.StartJobAsync(stoppingToken);
        await Task.Delay(10000, stoppingToken);
        await testResultPublisher.PublishAsync(
            new TestResultMessage("Ia ni ca merge"),
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