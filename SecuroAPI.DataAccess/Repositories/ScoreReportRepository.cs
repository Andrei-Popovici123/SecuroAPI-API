using Microsoft.EntityFrameworkCore;
using SecuroAPI.DataAccess.Entities;
using SecuroAPI.DataAccess.Repositories.Interfaces;

namespace SecuroAPI.DataAccess.Repositories;

public class ScoreReportRepository : BaseRepository<ScoreReport>, IScoreReportRepository
{
    private readonly SecuroAPIDbContext _dbContext;

    public ScoreReportRepository(SecuroAPIDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<ScoreReport>> GetAllByRatingId(Guid RatingId)
    {
        return await _dbContext.ScoreReports
            .Where(sr => sr.RatingId == RatingId)
            .ToListAsync();
    }
}