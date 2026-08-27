using Microsoft.EntityFrameworkCore;
using SecuroAPI.DataAccess.Entities;
using SecuroAPI.DataAccess.Repositories.Interfaces;

namespace SecuroAPI.DataAccess.Repositories;

public class TestJobRepository : BaseRepository<TestJob>, ITestJobRepository
{
    private readonly SecuroAPIDbContext _dbContext;

    public TestJobRepository(SecuroAPIDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<TestJob?> GetLatestForApiAsync(Guid apiId)
        => await _dbContext.TestJobs
            .AsNoTracking()
            .Where(j => j.APIID == apiId)
            .OrderByDescending(j => j.CreatedAt)
            .FirstOrDefaultAsync();
}