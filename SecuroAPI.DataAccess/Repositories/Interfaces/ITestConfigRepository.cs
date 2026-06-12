using SecuroAPI.DataAccess.Entities;

namespace SecuroAPI.DataAccess.Repositories.Interfaces;

public interface ITestConfigRepository: IRepository<TestConfig>
{
    Task<TestConfig?> GetByAPIID(Guid APIID);
}