using SecuroAPI.BusinessLogic.DTO_s.Monitoring;
using SecuroAPI.Common.Results;

namespace SecuroAPI.BusinessLogic.Services.Interfaces;

public interface IMonitoredEndpointService
{
    Task<Result<IEnumerable<MonitoredEndpointDto>>> GetAllByAPIID(Guid apiId);
    Task<Result<MonitoredEndpointDto>> GetByIdAsync(Guid id);
    Task<Result<MonitoredEndpointDto>> CreateAsync(Guid apiId, CreateMonitoredEndpointDto? endpointDto);
    Task<Result<MonitoredEndpointDto>> SetActiveAsync(Guid id, bool isActive);
    Task<Result> DeleteAsync(Guid id);
    Task<Result<ProbeResult>> ProbeNowAsync(Guid id, CancellationToken ct);
}