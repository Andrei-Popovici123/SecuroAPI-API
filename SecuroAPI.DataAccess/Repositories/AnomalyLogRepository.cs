using Microsoft.EntityFrameworkCore;
using SecuroAPI.DataAccess.Entities;
using SecuroAPI.DataAccess.Repositories.Interfaces;

namespace SecuroAPI.DataAccess.Repositories;

public class AnomalyLogRepository : BaseRepository<AnomalyLog>, IAnomalyLogRepository
{
    private readonly SecuroAPIDbContext _dbContext;

    public AnomalyLogRepository(SecuroAPIDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<AnomalyLog>> GetAllByAPIID(Guid APIID)
    {
        return await _dbContext.AnomalyLogs
            .Where(l=>l.APIID == APIID)
            .ToListAsync();
    }
}