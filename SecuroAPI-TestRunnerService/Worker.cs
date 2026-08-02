using System.Text;
using RabbitMQ.Client;

namespace SecuroAPI_TestRunnerService;

public class Worker(ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        await using var connection = await factory.CreateConnectionAsync(stoppingToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);
        {
            await channel.QueueDeclareAsync(queue: "test.jobs", durable: true,
                exclusive: false, autoDelete: false, arguments: null, cancellationToken: stoppingToken);
            string message = "Hello World from Test Job!";
            var body = Encoding.UTF8.GetBytes(message);
            
            await channel.BasicPublishAsync("", "test.jobs", body, cancellationToken: stoppingToken);
            Console.WriteLine("  Sent message {0}", message);
        }
        
        

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