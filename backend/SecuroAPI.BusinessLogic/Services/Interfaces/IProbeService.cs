using SecuroAPI.BusinessLogic.DTO_s.Monitoring;

namespace SecuroAPI.BusinessLogic.Services.Interfaces;

public interface IProbeService
{
    Task<ProbeResult> ProbeAsync(string url, CancellationToken ct = default);
}