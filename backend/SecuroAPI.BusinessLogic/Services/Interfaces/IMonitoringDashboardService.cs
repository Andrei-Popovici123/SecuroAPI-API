using SecuroAPI.BusinessLogic.DTO_s.Monitoring;
using SecuroAPI.Common.Results;

namespace SecuroAPI.BusinessLogic.Services.Interfaces;

public interface IMonitoringDashboardService
{
    Task<Result<MonitoringSummaryDto>> GetSummaryAsync(Guid apiId, int windowHours);
    Task<Result<TelemetrySeriesDto>> GetSeriesAsync(Guid endpointId, int windowHours);
}