
using System.Text.Json;
using RabbitMQ.Client;
using SecuroAPI.BusinessLogic.Services.Publisher;
using SecuroAPI.Contracts.Events;
using SecuroAPI.Contracts.Connection;
namespace SecuroAPI_API.Messaging;

public class TestJobPublisher : ITestJobPublisher
{
    private readonly RabbitMqConnection _connection;

    public TestJobPublisher(RabbitMqConnection connection)
    {
        _connection = connection;
    }

    public async Task PublishAsync(TestJobMessage message, CancellationToken cancellationToken = default)
    {
       await using var channel=await _connection.CreateChannelAsync(cancellationToken);

        await channel.QueueDeclareAsync("test.jobs", durable: true, exclusive: false,
            autoDelete: false, arguments: null, cancellationToken: cancellationToken);

        var body = JsonSerializer.SerializeToUtf8Bytes(message);

        await channel.BasicPublishAsync(
        exchange: "",
        routingKey: "test.jobs",
        mandatory:false,
        basicProperties: new BasicProperties { Persistent = true},
        body : body,
        cancellationToken: cancellationToken
            );
    }
}