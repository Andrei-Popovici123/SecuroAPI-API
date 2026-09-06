using Microsoft.EntityFrameworkCore;
using SecuroAPI.DataAccess.Entities;
using SecuroAPI.DataAccess.Repositories.Interfaces;

namespace SecuroAPI.DataAccess.Repositories;

public class MonitoredEndpointRepository : BaseRepository<MonitoredEndpoint>, IMonitoredEndpointRepository
{
    private readonly SecuroAPIDbContext _dbContext;

    public MonitoredEndpointRepository(SecuroAPIDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<IEnumerable<MonitoredEndpoint>> GetAllByAPIID(Guid apiId)
        => await _dbContext.MonitoredEndpoints
            .Where(e => e.APIID == apiId)
            .OrderBy(e => e.CreatedAt)
            .ToListAsync();

    public async Task<int> CountActiveByAPIID(Guid apiId)
        => await _dbContext.MonitoredEndpoints
            .CountAsync(e => e.APIID == apiId && e.IsActive);
    public async Task<List<MonitoredEndpoint>> GetDueAsync(DateTime now, int batchSize)
        => await _dbContext.MonitoredEndpoints
            .Where(e => e.IsActive &&
                        (e.LastCheckedAt == null ||
                         EF.Functions.DateDiffSecond(e.LastCheckedAt.Value, now) >= e.IntervalSeconds))
            .OrderBy(e => e.LastCheckedAt)
            .Take(batchSize)
            .ToListAsync();
}