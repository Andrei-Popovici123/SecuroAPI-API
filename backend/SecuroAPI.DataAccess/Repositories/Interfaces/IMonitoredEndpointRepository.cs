using SecuroAPI.DataAccess.Entities;

namespace SecuroAPI.DataAccess.Repositories.Interfaces;

public interface IMonitoredEndpointRepository : IRepository<MonitoredEndpoint>
{
    Task<IEnumerable<MonitoredEndpoint>> GetAllByAPIID(Guid apiId);
    Task<int> CountActiveByAPIID(Guid apiId);
}