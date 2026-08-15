using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SecuroAPI_MonitoringService.Messaging;
using SecuroAPI.Contracts.Events;

namespace SecuroAPI_MonitoringService;

public class Worker(ILogger<Worker> logger,MonitoringResultPublisher monitoringResultPublisher) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //
        // var factory = new ConnectionFactory() { HostName = "localhost" };
        // await using var connection = await factory.CreateConnectionAsync(stoppingToken);
        // await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);
        // {
        //     await channel.QueueDeclareAsync(queue: "test.jobs", durable: true,
        //         exclusive: false, autoDelete: false, arguments: null, cancellationToken: stoppingToken);
        //     var consumer = new AsyncEventingBasicConsumer(channel);
        //     consumer.ReceivedAsync += async (model, ea) =>
        //     {
        //         var body = ea.Body;
        //         var message = Encoding.UTF8.GetString(body.ToArray());
        //         Console.WriteLine($"Received Message ... {message}", message);
        //     };
        //     
        //     await channel.BasicConsumeAsync("test.jobs", true, consumer, cancellationToken: stoppingToken);
        // }
        
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
