using SecuroAPI.BusinessLogic.DTO_s.Monitoring;
using SecuroAPI.BusinessLogic.Services.Interfaces;
using SecuroAPI.Common.Constants;
using SecuroAPI.Common.Results;
using SecuroAPI.Common.UrlNormalizer;
using SecuroAPI.DataAccess.Entities;
using SecuroAPI.DataAccess.Repositories.Interfaces;

namespace SecuroAPI.BusinessLogic.Services;

public class MonitoredEndpointService : IMonitoredEndpointService
{
    private const int MaxEndpointsPerApi = 5;
    private const int ManualProbeCooldownSeconds = 30;

    private readonly IMonitoredEndpointRepository _repository;
    private readonly IAPIRegistryRepository _apiRepository;
    private readonly ITelemetryService _telemetryService;
    private readonly IProbeService _probeService;
    private readonly IUserService _userService;

    public MonitoredEndpointService(
        IMonitoredEndpointRepository repository,
        IAPIRegistryRepository apiRepository,
        ITelemetryService telemetryService,
        IProbeService probeService,
        IUserService userService)
    {
        _repository = repository;
        _apiRepository = apiRepository;
        _telemetryService = telemetryService;
        _probeService = probeService;
        _userService = userService;
    }
    
    public async Task<Result<IEnumerable<MonitoredEndpointDto>>> GetAllByAPIID(Guid apiId)
    {
        if (!await OwnsApi(apiId))
            return Result<IEnumerable<MonitoredEndpointDto>>.Failure(
                new Error(ErrorCodes.NotFound, $"API with the Id '{apiId}' was not found"));

        var endpoints = await _repository.GetAllByAPIID(apiId);

        return Result<IEnumerable<MonitoredEndpointDto>>.Success(endpoints.Select(MapToDto));
    }

    public async Task<Result<MonitoredEndpointDto>> GetByIdAsync(Guid id)
    {
        var endpoint = await _repository.GetByIdAsync(id);

        if (endpoint is null || !await OwnsApi(endpoint.APIID))
            return Result<MonitoredEndpointDto>.Failure(
                new Error(ErrorCodes.NotFound, $"Endpoint with the Id '{id}' was not found"));

        return Result<MonitoredEndpointDto>.Success(MapToDto(endpoint));
    }

    public async Task<Result<MonitoredEndpointDto>> CreateAsync(
        Guid apiId, CreateMonitoredEndpointDto? endpointDto)
    {
        if (endpointDto is null)
            return Result<MonitoredEndpointDto>.BadRequest(
                new Error(ErrorCodes.Validation, "Request body is required"));

        var api = await _apiRepository.GetAsync(
            a => a.APIID == apiId && a.UserID == _userService.UserId);

        if (api is null)
            return Result<MonitoredEndpointDto>.Failure(
                new Error(ErrorCodes.NotFound, $"API with the Id '{apiId}' was not found"));

        if (api.VerifiedAt is null)
            return Result<MonitoredEndpointDto>.Failure(
                new Error(ErrorCodes.Forbidden,
                    "Target must be domain-verified before endpoints can be monitored"));

        if (!UrlNormalizer.TryNormalize(endpointDto.Url, out var uri))
            return Result<MonitoredEndpointDto>.BadRequest(
                new Error(ErrorCodes.Validation,
                    "URL must be an absolute http(s) URL and not a bare IP address"));

        var verifiedHost = new Uri(api.TargetURL).Host;

        if (!string.Equals(uri.Host, verifiedHost, StringComparison.OrdinalIgnoreCase))
            return Result<MonitoredEndpointDto>.BadRequest(
                new Error(ErrorCodes.Validation,
                    $"Endpoint must be on the verified host '{verifiedHost}'"));

        if (await _repository.CountActiveByAPIID(apiId) >= MaxEndpointsPerApi)
            return Result<MonitoredEndpointDto>.Failure(
                new Error(ErrorCodes.Conflict,
                    $"Limit of {MaxEndpointsPerApi} monitored endpoints per target reached"));

        var canonical = UrlNormalizer.Canonical(uri);

        if (await _repository.CheckExistsAsync(e => e.APIID == apiId && e.Url == canonical))
            return Result<MonitoredEndpointDto>.Failure(
                new Error(ErrorCodes.Conflict, "This endpoint is already monitored"));

        var newEndpoint = await _repository.AddAsync(new MonitoredEndpoint
        {
            APIID           = apiId,
            Url             = canonical,
            Label           = endpointDto.Label,
            IntervalSeconds = 60,
            IsActive        = true,
            CreatedAt       = DateTime.UtcNow
        });

        return Result<MonitoredEndpointDto>.Success(MapToDto(newEndpoint));
    }

    public async Task<Result<MonitoredEndpointDto>> SetActiveAsync(Guid id, bool isActive)
    {
        var endpoint = await _repository.GetByIdAsync(id);

        if (endpoint is null || !await OwnsApi(endpoint.APIID))
            return Result<MonitoredEndpointDto>.Failure(
                new Error(ErrorCodes.NotFound, $"Endpoint with the Id '{id}' was not found"));

        if (isActive && !endpoint.IsActive
            && await _repository.CountActiveByAPIID(endpoint.APIID) >= MaxEndpointsPerApi)
            return Result<MonitoredEndpointDto>.Failure(
                new Error(ErrorCodes.Conflict,
                    $"Limit of {MaxEndpointsPerApi} active endpoints reached"));

        endpoint.IsActive       = isActive;
        endpoint.LastModifiedAt = DateTime.UtcNow;

        var updatedEndpoint = await _repository.UpdateAsync(endpoint);

        return Result<MonitoredEndpointDto>.Success(MapToDto(updatedEndpoint));
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var endpoint = await _repository.GetByIdAsync(id);

        if (endpoint is null || !await OwnsApi(endpoint.APIID))
            return Result.Failure(
                new Error(ErrorCodes.NotFound, $"Endpoint with the Id '{id}' was not found"));

        await _repository.DeleteAsync(id);

        return Result.Success();
    }

    public async Task<Result<ProbeResult>> ProbeNowAsync(Guid id, CancellationToken ct)
    {
        var endpoint = await _repository.GetByIdAsync(id);

        if (endpoint is null || !await OwnsApi(endpoint.APIID))
            return Result<ProbeResult>.Failure(
                new Error(ErrorCodes.NotFound, $"Endpoint with the Id '{id}' was not found"));

        var now = DateTime.UtcNow;

        if (endpoint.LastCheckedAt is { } lastChecked
            && (now - lastChecked).TotalSeconds < ManualProbeCooldownSeconds)
            return Result<ProbeResult>.Failure(
                new Error(ErrorCodes.Conflict,
                    $"Please wait {ManualProbeCooldownSeconds} seconds between manual probes"));

        var result = await _probeService.ProbeAsync(endpoint.Url, ct);

        await _telemetryService.RecordAsync(endpoint, result, now);

        return Result<ProbeResult>.Success(result);
    }
    
    private async Task<bool> OwnsApi(Guid apiId)
        => await _apiRepository.CheckExistsAsync(
            a => a.APIID == apiId && a.UserID == _userService.UserId);

    private static MonitoredEndpointDto MapToDto(MonitoredEndpoint e) => new()
    {
        EndpointId      = e.EndpointId,
        APIID           = e.APIID,
        Url             = e.Url,
        Label           = e.Label,
        IsActive        = e.IsActive,
        IntervalSeconds = e.IntervalSeconds,
        LastCheckedAt   = e.LastCheckedAt,
        LastOk          = e.LastOk,
        LastStatusCode  = e.LastStatusCode,
        LastLatencyMs   = e.LastLatencyMs,
        LastErrorType   = e.LastErrorType,
        HasHsts         = e.HasHsts,
        HasCsp          = e.HasCsp,
        HasNosniff      = e.HasNosniff,
        HasFrameOptions = e.HasFrameOptions,
        CreatedAt       = e.CreatedAt
    };

}