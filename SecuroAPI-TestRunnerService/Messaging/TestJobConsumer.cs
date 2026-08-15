using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SecuroAPI.Contracts.Connection;
using SecuroAPI.Contracts.Events;

namespace SecuroAPI_TestRunnerService.Messaging;

public class TestJobConsumer
{
    private readonly RabbitMqConnection _connection;
    private readonly ILogger<Worker> _logger;

    public TestJobConsumer(RabbitMqConnection connection, ILogger<Worker> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    public async Task StartJobAsync(CancellationToken cancellationToken)
    {
        var channel = await _connection.CreateChannelAsync(cancellationToken);
        await channel.QueueDeclareAsync("test.jobs", durable: true, exclusive: false, autoDelete: false,
            arguments: null, cancellationToken: cancellationToken);
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            var json = Encoding.UTF8.GetString(ea.Body.ToArray());
            var message = JsonSerializer.Deserialize<TestJobMessage>(json);

            _logger.LogInformation("Received job for: {MessageTargetUrl}", message?.TargetUrl);

            await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
        };
        await channel.BasicConsumeAsync("test.jobs", autoAck: false, 
            consumer, cancellationToken: cancellationToken);
    }
}