using System.Text.Json;
using RabbitMQ.Client;
using SecuroAPI.Contracts.Connection;
using SecuroAPI.Contracts.Events;

namespace SecuroAPI_TestRunnerService.Messaging;

public class TestResultPublisher
{
    private readonly RabbitMqConnection _connection;

    public TestResultPublisher(RabbitMqConnection connection)
    {
        _connection = connection;
    }

    public async Task PublishAsync(TestResultMessage message, CancellationToken cancellationToken = default)
    {
       await using var channel=await _connection.CreateChannelAsync(cancellationToken);

        await channel.QueueDeclareAsync("test.results", durable: true, exclusive: false,
            autoDelete: false, arguments: null, cancellationToken: cancellationToken);

        var body = JsonSerializer.SerializeToUtf8Bytes(message);

        await channel.BasicPublishAsync(
        exchange: "",
        routingKey: "test.results",
        mandatory:false,
        basicProperties: new BasicProperties { Persistent = true},
        body : body,
        cancellationToken: cancellationToken
            );
    }
}