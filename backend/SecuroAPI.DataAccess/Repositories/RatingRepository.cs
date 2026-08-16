using Microsoft.EntityFrameworkCore;
using SecuroAPI.DataAccess.Entities;
using SecuroAPI.DataAccess.Repositories.Interfaces;

namespace SecuroAPI.DataAccess.Repositories;

public class RatingRepository : BaseRepository<Rating>, IRatingRepository
{
    private readonly SecuroAPIDbContext _dbContext;

    public RatingRepository(SecuroAPIDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Rating>> GetAllByAPIID(Guid APIID)
    {
        return await _dbContext.Ratings
            .Where(r=>r.APIID == APIID)
            .ToListAsync();
    }
}