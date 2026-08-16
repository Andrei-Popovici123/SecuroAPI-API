using Microsoft.EntityFrameworkCore;
using SecuroAPI.DataAccess.Entities;
using SecuroAPI.DataAccess.Repositories.Interfaces;

namespace SecuroAPI.DataAccess.Repositories;

public class TestConfigRepository : BaseRepository<TestConfig>, ITestConfigRepository
{
    private readonly SecuroAPIDbContext _dbContext;

    public TestConfigRepository(SecuroAPIDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TestConfig?> GetByAPIID(Guid APIID)
    {
        return await _dbContext.TestConfigs
            .FirstOrDefaultAsync(r => r.APIID == APIID);

    }
}