using SecuroAPI.BusinessLogic.DTO_s.Monitoring;
using SecuroAPI.BusinessLogic.Services.Interfaces;
using SecuroAPI.DataAccess.Entities;
using SecuroAPI.DataAccess.Repositories.Interfaces;

namespace SecuroAPI.BusinessLogic.Services;

public class TelemetryService : ITelemetryService
{
    private readonly IMonitoredEndpointRepository _endpointRepository;
    private readonly ITelemetryRepository _telemetryRepository;

    public TelemetryService(IMonitoredEndpointRepository endpointRepository, ITelemetryRepository telemetryRepository)
    {
        _endpointRepository = endpointRepository;
        _telemetryRepository = telemetryRepository;
    }
    
    public async Task RecordAsync(MonitoredEndpoint endpoint, ProbeResult result, DateTime checkedAt)
    {
        endpoint.LastCheckedAt   = checkedAt;
        endpoint.LastOk          = result.Ok;
        endpoint.LastStatusCode  = result.StatusCode;
        endpoint.LastLatencyMs   = result.LatencyMs;
        endpoint.LastErrorType   = result.ErrorType;
        endpoint.HasHsts         = result.Hsts;
        endpoint.HasCsp          = result.Csp;
        endpoint.HasNosniff      = result.Nosniff;
        endpoint.HasFrameOptions = result.FrameOptions;

        await _endpointRepository.UpdateAsync(endpoint);

        await _telemetryRepository.AddAsync(new TelemetryPoint
        {
            EndpointId = endpoint.EndpointId,
            CheckedAt  = checkedAt,
            Ok         = result.Ok,
            StatusCode = result.StatusCode,
            LatencyMs  = result.LatencyMs,
            ErrorType  = result.ErrorType
        });
    }
}