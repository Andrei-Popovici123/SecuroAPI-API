using SecuroAPI.BusinessLogic.DTO_s.Monitoring;
using SecuroAPI.BusinessLogic.Services.Interfaces;
using SecuroAPI.Common.Constants;
using SecuroAPI.Common.Results;
using SecuroAPI.DataAccess.Repositories.Interfaces;

namespace SecuroAPI.BusinessLogic.Services;

public class MonitoringDashboardService : IMonitoringDashboardService
{
    private const int MaxSummaryWindowHours = 720;
    private const int MaxSeriesWindowHours = 168;

    private readonly IMonitoredEndpointRepository _endpointRepository;
    private readonly ITelemetryRepository _telemetryRepository;
    private readonly IAPIRegistryRepository _apiRepository;
    private readonly IUserService _userService;

    public MonitoringDashboardService(
        IMonitoredEndpointRepository endpointRepository,
        ITelemetryRepository telemetryRepository,
        IAPIRegistryRepository apiRepository,
        IUserService userService)
    {
        _endpointRepository = endpointRepository;
        _telemetryRepository = telemetryRepository;
        _apiRepository = apiRepository;
        _userService = userService;
    }

    private async Task<bool> OwnsApi(Guid apiId)
        => await _apiRepository.CheckExistsAsync(
            a => a.APIID == apiId && a.UserID == _userService.UserId);

    public async Task<Result<MonitoringSummaryDto>> GetSummaryAsync(Guid apiId, int windowHours)
    {
        if (!await OwnsApi(apiId))
            return Result<MonitoringSummaryDto>.Failure(
                new Error(ErrorCodes.NotFound, $"API with the Id '{apiId}' was not found"));

        if (windowHours < 1 || windowHours > MaxSummaryWindowHours)
            return Result<MonitoringSummaryDto>.BadRequest(
                new Error(ErrorCodes.Validation,
                    $"windowHours must be between 1 and {MaxSummaryWindowHours}"));

        var since = DateTime.UtcNow.AddHours(-windowHours);

        var endpoints = (await _endpointRepository.GetAllByAPIID(apiId)).ToList();
        var points = await _telemetryRepository.GetAllByAPIIDSince(apiId, since);

        var summary = new MonitoringSummaryDto
        {
            APIID               = apiId,
            EndpointCount       = endpoints.Count,
            ActiveEndpointCount = endpoints.Count(e => e.IsActive),
            EndpointsUp         = endpoints.Count(e => e.LastOk == true),
            EndpointsDown       = endpoints.Count(e => e.LastOk == false),
            LastCheckedAt       = endpoints.Count > 0
                                    ? endpoints.Max(e => e.LastCheckedAt)
                                    : null,
            ProbeCount          = points.Count,
            WindowHours         = windowHours
        };

        if (points.Count == 0)
            return Result<MonitoringSummaryDto>.Success(summary);

        summary.UptimePercent =
            Math.Round(points.Count(p => p.Ok) * 100.0 / points.Count, 2);

        var successful = points
            .Where(p => p.Ok)
            .Select(p => p.LatencyMs)
            .OrderBy(l => l)
            .ToList();

        if (successful.Count > 0)
        {
            summary.AvgLatencyMs = (int)successful.Average();
            summary.P95LatencyMs = successful[(int)Math.Ceiling(successful.Count * 0.95) - 1];
        }

        return Result<MonitoringSummaryDto>.Success(summary);
    }

    public async Task<Result<TelemetrySeriesDto>> GetSeriesAsync(Guid endpointId, int windowHours)
    {
        var endpoint = await _endpointRepository.GetByIdAsync(endpointId);

        if (endpoint is null || !await OwnsApi(endpoint.APIID))
            return Result<TelemetrySeriesDto>.Failure(
                new Error(ErrorCodes.NotFound, $"Endpoint with the Id '{endpointId}' was not found"));

        if (windowHours < 1 || windowHours > MaxSeriesWindowHours)
            return Result<TelemetrySeriesDto>.BadRequest(
                new Error(ErrorCodes.Validation,
                    $"windowHours must be between 1 and {MaxSeriesWindowHours}"));

        var since = DateTime.UtcNow.AddHours(-windowHours);

        var points = await _telemetryRepository.GetSeriesAsync(endpointId, since);

        return Result<TelemetrySeriesDto>.Success(new TelemetrySeriesDto
        {
            EndpointId  = endpoint.EndpointId,
            Label       = endpoint.Label,
            Url         = endpoint.Url,
            WindowHours = windowHours,
            Points      = points.Select(p => new TelemetrySeriesPointDto
            {
                CheckedAt  = p.CheckedAt,
                LatencyMs  = p.LatencyMs,
                Ok         = p.Ok,
                StatusCode = p.StatusCode
            }).ToList()
        });
    }
}