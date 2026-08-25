using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SecuroAPI_TestRunnerService.Logic.Interfaces;
using SecuroAPI.Contracts.Connection;
using SecuroAPI.Contracts.Events;

namespace SecuroAPI_TestRunnerService.Messaging;

public class TestJobConsumer
{
    private readonly RabbitMqConnection _connection;
    private readonly IContainerRunner _containerRunner;
    private readonly ILogger<TestJobConsumer> _logger;
    private readonly TestResultPublisher _publisher;
    private IChannel? _channel;

    public TestJobConsumer(RabbitMqConnection connection, ILogger<TestJobConsumer> logger,
        TestResultPublisher publisher, IContainerRunner containerRunner)
    {
        _connection = connection;
        _logger = logger;
        _publisher = publisher;
        _containerRunner = containerRunner;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _channel = await _connection.CreateChannelAsync(cancellationToken);

        await _channel.QueueDeclareAsync("test.jobs", durable: true, exclusive: false, autoDelete: false,
            arguments: null, cancellationToken: cancellationToken);

        await _channel.BasicQosAsync(0, prefetchCount: 1, global: false, cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += OnMessageAsync;
        await _channel.BasicConsumeAsync("test.jobs", autoAck: false, consumer, cancellationToken);
    }

    private async Task OnMessageAsync(object sender, BasicDeliverEventArgs ea)
    {
        var json = Encoding.UTF8.GetString(ea.Body.Span);
        try
        {
            var job = JsonSerializer.Deserialize<TestJobMessage>(json);
            _logger.LogInformation("Received job for {APIID} -> {TargetUrl}", job?.APIID, job?.TargetUrl);

            if (job != null)
            {
                var (exitCode, stdout) = await _containerRunner.RunAsync(job.TargetUrl, ea.CancellationToken);

                await _publisher.PublishAsync(new TestResultMessage(job.APIID, exitCode, stdout));
            }

            await _channel!.BasicAckAsync(ea.DeliveryTag, multiple: false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Job failed: {Json}", json);
            await _channel!.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
        }
    }
}