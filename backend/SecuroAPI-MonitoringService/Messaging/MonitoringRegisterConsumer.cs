using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SecuroAPI.Contracts.Connection;
using SecuroAPI.Contracts.Events;

namespace SecuroAPI_MonitoringService.Messaging;

public class MonitoringRegisterConsumer
{
    private readonly RabbitMqConnection _connection;
    private readonly ILogger<MonitoringWorker> _logger;

    public MonitoringRegisterConsumer(RabbitMqConnection connection, ILogger<MonitoringWorker> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    public async Task StartJobAsync(CancellationToken cancellationToken)
    {
        var channel = await _connection.CreateChannelAsync(cancellationToken);
        await channel.QueueDeclareAsync("monitoring.register", durable: true, exclusive: false, autoDelete: false,
            arguments: null, cancellationToken: cancellationToken);
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            var json = Encoding.UTF8.GetString(ea.Body.ToArray());
            var message = JsonSerializer.Deserialize<RegisterMonitoringMessage>(json);

            _logger.LogInformation("Registered URL for monitoring: {MessageTargetUrl}", message?.Message);

            await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
        };
        await channel.BasicConsumeAsync("monitoring.register", autoAck: false, 
            consumer, cancellationToken: cancellationToken);
    }
}