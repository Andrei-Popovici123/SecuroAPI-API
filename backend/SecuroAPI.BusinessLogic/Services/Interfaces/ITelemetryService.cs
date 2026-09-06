using SecuroAPI.BusinessLogic.DTO_s.Monitoring;
using SecuroAPI.DataAccess.Entities;

namespace SecuroAPI.BusinessLogic.Services.Interfaces;

public interface ITelemetryService
{
    Task RecordAsync(MonitoredEndpoint endpoint, ProbeResult result, DateTime checkedAt);
}