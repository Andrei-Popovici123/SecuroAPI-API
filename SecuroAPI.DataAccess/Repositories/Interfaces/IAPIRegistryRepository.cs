using SecuroAPI.DataAccess.Entities;

namespace SecuroAPI.DataAccess.Repositories.Interfaces;

public interface IAPIRegistryRepository: IRepository<APIRegistry>
{
    Task<IEnumerable<APIRegistry>> GetAllById(string id);
}