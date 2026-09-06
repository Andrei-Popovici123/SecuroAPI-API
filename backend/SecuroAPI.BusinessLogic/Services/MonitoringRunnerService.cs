using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SecuroAPI.BusinessLogic.Services.Interfaces;
using SecuroAPI.Common.Constants;
using SecuroAPI.DataAccess.Repositories.Interfaces;

namespace SecuroAPI.BusinessLogic.Services;

public class MonitoringRunnerService:  IMonitoringRunnerService
{
    private readonly IMonitoredEndpointRepository _repository;
    private readonly IProbeService _probeService;
    private readonly ITelemetryService _telemetryService;
    private readonly MonitoringOptions _options;
    private readonly ILogger<MonitoringRunnerService> _logger;

    public MonitoringRunnerService(
        IMonitoredEndpointRepository repository,
        IProbeService probeService,
        ITelemetryService telemetryService,
        IOptions<MonitoringOptions> options,
        ILogger<MonitoringRunnerService> logger)
    {
        _repository = repository;
        _probeService = probeService;
        _telemetryService = telemetryService;
        _options = options.Value;
        _logger = logger;
    }

    public async Task RunDueAsync(CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        var due = await _repository.GetDueAsync(now, _options.BatchSize);

        if (due.Count == 0) return;

        if (due.Count == _options.BatchSize)
            _logger.LogWarning(
                "Monitoring batch saturated at {BatchSize} — endpoints may be drifting past their interval",
                _options.BatchSize);
        
        using var gate = new SemaphoreSlim(_options.Concurrency);

        var probes = due.Select(async endpoint =>
        {
            await gate.WaitAsync(ct);
            try
            {
                var result = await _probeService.ProbeAsync(endpoint.Url, ct);
                return (endpoint, result);
            }
            finally
            {
                gate.Release();
            }
        });

        var results = await Task.WhenAll(probes);
        
        foreach (var (endpoint, result) in results)
        {
            await _telemetryService.RecordAsync(endpoint, result, DateTime.UtcNow);

            if (!result.Ok)
                _logger.LogInformation(
                    "Probe failed {EndpointId} {Url} {ErrorType} {StatusCode}",
                    endpoint.EndpointId, endpoint.Url, result.ErrorType, result.StatusCode);
        }

        _logger.LogInformation(
            "Monitoring tick complete: {Total} probed, {Failed} failed",
            results.Length, results.Count(r => !r.result.Ok));
    }
}
