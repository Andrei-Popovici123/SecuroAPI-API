using Microsoft.EntityFrameworkCore;
using SecuroAPI.DataAccess.Entities;
using SecuroAPI.DataAccess.Repositories.Interfaces;

namespace SecuroAPI.DataAccess.Repositories;

public class TelemetryRepository : ITelemetryRepository
{
    private readonly SecuroAPIDbContext _dbContext;
    public TelemetryRepository(SecuroAPIDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task AddAsync(TelemetryPoint point)
    {
        await _dbContext.TelemetryPoints.AddAsync(point);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<TelemetryPoint>> GetSeriesAsync(Guid endpointId, DateTime since)
        => await _dbContext.TelemetryPoints
            .Where(t => t.EndpointId == endpointId && t.CheckedAt >= since)
            .OrderBy(t => t.CheckedAt)
            .AsNoTracking()
            .ToListAsync();
    public async Task<List<TelemetryPoint>> GetAllByAPIIDSince(Guid apiId, DateTime since)
        => await _dbContext.TelemetryPoints
            .Where(t => t.Endpoint.APIID == apiId && t.CheckedAt >= since)
            .AsNoTracking()
            .ToListAsync();
    
}