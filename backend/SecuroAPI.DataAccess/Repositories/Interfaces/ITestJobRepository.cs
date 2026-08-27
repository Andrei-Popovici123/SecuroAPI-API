using SecuroAPI.DataAccess.Entities;

namespace SecuroAPI.DataAccess.Repositories.Interfaces;

public interface ITestJobRepository : IRepository<TestJob>
{
    Task<TestJob?> GetLatestForApiAsync(Guid apiId);
}