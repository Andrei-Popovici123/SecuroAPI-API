using System.Text;
using RabbitMQ.Client;
using SecuroAPI_TestRunnerService.Messaging;
using SecuroAPI.Contracts.Events;

namespace SecuroAPI_TestRunnerService;

public class Worker(ILogger<Worker> logger, TestJobConsumer consumer) : BackgroundService
{
 

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    { 
        

        await consumer.StartAsync(stoppingToken);
        logger.LogInformation("Listening on test.jobs");

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}