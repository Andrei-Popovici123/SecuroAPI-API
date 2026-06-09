using SecuroAPI.DataAccess.Entities;

namespace SecuroAPI.DataAccess.Repositories.Interfaces;

public interface IAnomalyLogRepository: IRepository<AnomalyLog>
{
    Task<IEnumerable<AnomalyLog>> GetAllByAPIID(Guid APIID);
}