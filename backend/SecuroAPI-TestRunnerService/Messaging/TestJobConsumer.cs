using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SecuroAPI_TestRunnerService.Logic.Interfaces;
using SecuroAPI.Common.Enums;
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
    private const int MaxConcurrent = 3;
    private const int ScanTimeoutInMinutes = 1;
    private readonly SemaphoreSlim _slots = new(MaxConcurrent, MaxConcurrent);

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

        await _channel.BasicQosAsync(0, prefetchCount: MaxConcurrent, global: false, cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += OnMessageAsync;
        await _channel.BasicConsumeAsync("test.jobs", autoAck: false, consumer, cancellationToken);
    }

    private async Task OnMessageAsync(object sender, BasicDeliverEventArgs ea)
    {
        var json = Encoding.UTF8.GetString(ea.Body.Span);

        try
        {
            await _slots.WaitAsync(ea.CancellationToken);
        }
        // consumer shut down while waiting for a slot — never processed,
        // so no ack/nack: Rabbit requeues on channel close
        catch (OperationCanceledException)
        {
            return;
        }
        Guid? jobId = null;
        try
        {
            var job = JsonSerializer.Deserialize<TestJobMessage>(json);

            if (job is null)
                throw new JsonException("Null TestJobMessage");
            
            jobId = job.JobId;
            
            _logger.LogInformation("Received job {JobId} for {APIID} -> {TargetUrl}",
                job.JobId, job.APIID, job.TargetUrl);

            await _publisher.PublishAsync(
                new TestJobStatusMessage(job.JobId, JobStatus.Running, DateTime.UtcNow),
                ea.CancellationToken);

            //Job Timeout Currently set to 5 minutes
            using var scanCts = CancellationTokenSource.CreateLinkedTokenSource(ea.CancellationToken);
            scanCts.CancelAfter(TimeSpan.FromMinutes(ScanTimeoutInMinutes));
            
            var (exitCode, stdout) = await _containerRunner.RunAsync(job.TargetUrl, scanCts.Token);

            await _publisher.PublishAsync(new TestResultMessage(job.JobId, job.APIID, exitCode, stdout),ea.CancellationToken);


            await _channel!.BasicAckAsync(ea.DeliveryTag, multiple: false);
        }
        catch (OperationCanceledException) when (ea.CancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("Shutdown during job {JobId}", jobId);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Job {JobId} timed out after {Minutes}m", jobId, ScanTimeoutInMinutes);
            await PublishFailedAsync(jobId, $"Scan exceeded {ScanTimeoutInMinutes} minute timeout");
            await _channel!.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Job failed: {Json}", json);
            await PublishFailedAsync(jobId, ex.Message);
            await _channel!.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
        }
        finally
        {
            _slots.Release();
        }
    }

    private async Task PublishFailedAsync(Guid? jobId, string error)
    {
        if (jobId is null) return;

        try
        {
            await _publisher.PublishAsync(
                new TestJobStatusMessage(jobId.Value, JobStatus.Failed, DateTime.UtcNow, error));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not publish Failed for {JobId}", jobId);
        }
    }
}