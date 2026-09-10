using RabbitMQ.Client;

namespace SecuroAPI.Contracts.Connection;

public class RabbitMqConnection : IAsyncDisposable
{
    private readonly ConnectionFactory _connectionFactory = new()
    {
        HostName = Environment.GetEnvironmentVariable("RabbitMq__Host") ?? "localhost",
        UserName = Environment.GetEnvironmentVariable("RabbitMq__Username") ?? "guest",
        Password = Environment.GetEnvironmentVariable("RabbitMq__Password") ?? "guest"
    };

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

    public async Task<IChannel> CreateChannelAsync(CancellationToken cancellationToken, CreateChannelOptions? options = null)
    {
        var connection = await GetConnectionAsync(cancellationToken);
        return await connection.CreateChannelAsync(cancellationToken: cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null) await _connection.DisposeAsync();
        _connectionLock.Dispose();
    }
}