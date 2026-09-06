using SecuroAPI.DataAccess.Entities;

namespace SecuroAPI.DataAccess.Repositories.Interfaces;

public interface ITelemetryRepository
{
    Task AddAsync(TelemetryPoint point);
    Task<IEnumerable<TelemetryPoint>> GetSeriesAsync(Guid endpointId, DateTime since);
}