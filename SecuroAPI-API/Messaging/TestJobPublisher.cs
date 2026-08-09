
using System.Text.Json;
using RabbitMQ.Client;
using SecuroAPI.BusinessLogic.Services.Publisher;
using SecuroAPI.Contracts.Events;

namespace SecuroAPI_API.Messaging;

public class TestJobPublisher : ITestJobPublisher, IAsyncDisposable
{
    private readonly ConnectionFactory _connectionFactory = new() { HostName = "localhost" };
    private IConnection? _connection;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);

    private async Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken)
    {
        if (_connection is { IsOpen: true }) return _connection;
        await _connectionLock.WaitAsync(cancellationToken);
        try
        {
            if (_connection is { IsOpen: true }) return _connection;
            _connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
            return _connection;
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async Task PublishAsync(TestJobMessage message, CancellationToken cancellationToken = default)
    {
        var connection = await GetConnectionAsync(cancellationToken);
        await using var chanel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await chanel.QueueDeclareAsync("test.jobs", durable: true, exclusive: false,
            autoDelete: false, arguments: null, cancellationToken: cancellationToken);

        var body = JsonSerializer.SerializeToUtf8Bytes(message);

        await chanel.BasicPublishAsync(
        exchange: "",
        routingKey: "test.jobs",
        mandatory:false,
        basicProperties: new BasicProperties { Persistent = true},
        body : body,
        cancellationToken: cancellationToken
            );
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null) await _connection.DisposeAsync();
    }
}