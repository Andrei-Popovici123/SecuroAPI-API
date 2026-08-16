using SecuroAPI.DataAccess.Entities;

namespace SecuroAPI.DataAccess.Repositories.Interfaces;

public interface IRatingRepository: IRepository<Rating>
{
    Task<IEnumerable<Rating>> GetAllByAPIID(Guid APIID);
}