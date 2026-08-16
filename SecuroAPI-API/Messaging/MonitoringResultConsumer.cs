using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SecuroAPI.Contracts.Connection;
using SecuroAPI.Contracts.Events;

namespace SecuroAPI_API.Messaging;

public class MonitoringResultConsumer : BackgroundService
{
    private readonly RabbitMqConnection _connection;
    private readonly ILogger<MonitoringResultConsumer> _logger;
    private IChannel? _channel;
    
    public MonitoringResultConsumer(RabbitMqConnection connection, ILogger<MonitoringResultConsumer> logger)
    {
        _connection = connection;
        _logger = logger;
    }

   protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    { 
        _channel = await _connection.CreateChannelAsync(cancellationToken);
        await _channel.QueueDeclareAsync("monitoring.results", durable: true, exclusive: false, autoDelete: false,
            arguments: null, cancellationToken: cancellationToken);
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            var json = Encoding.UTF8.GetString(ea.Body.ToArray());
            var message = JsonSerializer.Deserialize<MonitoringMessage>(json);

            _logger.LogInformation("Registered URL for monitoring: {MessageTargetUrl}", message?.Message);

            // TODO(persistence): open a scope here, resolve a scoped service, save the result
            // so the user can retrieve it later. This is where IServiceScopeFactory + DbContext
            // come in — deferred until the API actually stores results.
            
            await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
        };
        await _channel.BasicConsumeAsync("monitoring.results", autoAck: false, 
            consumer, cancellationToken: cancellationToken);
    }
}