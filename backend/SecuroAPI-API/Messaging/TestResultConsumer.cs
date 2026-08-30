using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SecuroAPI.BusinessLogic.DTO_s.TestRun;
using SecuroAPI.BusinessLogic.Services.Interfaces;
using SecuroAPI.Contracts.Connection;
using SecuroAPI.Contracts.Events;

namespace SecuroAPI_API.Messaging;

public class TestResultConsumer : BackgroundService
{
    private readonly RabbitMqConnection _connection;
    private readonly ILogger<TestResultConsumer> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private IChannel? _channel;

    public TestResultConsumer(RabbitMqConnection connection, ILogger<TestResultConsumer> logger,
        IServiceScopeFactory scopeFactory)
    {
        _connection = connection;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _channel = await _connection.CreateChannelAsync(cancellationToken);

        await _channel.QueueDeclareAsync("test.results", durable: true, exclusive: false, autoDelete: false,
            arguments: null, cancellationToken: cancellationToken);

        await _channel.BasicQosAsync(0, prefetchCount: 5, global: false, cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            var json = Encoding.UTF8.GetString(ea.Body.Span);

            try
            {
                var msg = JsonSerializer.Deserialize<RunnerMessage>(json);
                switch (msg)
                {
                    case TestJobStatusMessage s:
                        await HandleStatusAsync(s, cancellationToken);
                        break;

                    case TestResultMessage r:
                        await HandleResultAsync(r, cancellationToken);
                        break;

                    default:
                        _logger.LogWarning("Unknown runner message: {Json}", json);
                        break;
                }

                await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
            }


            catch (Exception ex)
            {
                _logger.LogError(ex, "Result message failed: {Json}", json);
                await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
            }
        };
        await _channel.BasicConsumeAsync("test.results", autoAck: false,
            consumer, cancellationToken: cancellationToken);
    }

    private async Task HandleResultAsync(TestResultMessage testResultMessage, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var jobService = scope.ServiceProvider.GetRequiredService<ITestJobService>();

        var result = await jobService.SaveResultAsync(new JobResultDto(testResultMessage.JobId,
            testResultMessage.APIID, testResultMessage.ExitCode, testResultMessage.Output));
        if (!result.IsSuccess)
            _logger.LogWarning("Result for job {JobId} not persisted: {Error}",
                testResultMessage.JobId, result.Errors.FirstOrDefault().Description);
    }

    private async Task HandleStatusAsync(TestJobStatusMessage testJobStatusMessage,
        CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var jobService = scope.ServiceProvider.GetRequiredService<ITestJobService>();

        var result = await jobService.ApplyStatusAsync(new JobStatusDto(testJobStatusMessage.JobId, testJobStatusMessage.Status,
            testJobStatusMessage.OccurredAt,testJobStatusMessage.Error));
        if (!result.IsSuccess)
            _logger.LogWarning("Status {Status} for job {JobId} not applied: {Error}",
                testJobStatusMessage.Status, testJobStatusMessage.JobId,
                result.Errors.FirstOrDefault().Description);
    }
}