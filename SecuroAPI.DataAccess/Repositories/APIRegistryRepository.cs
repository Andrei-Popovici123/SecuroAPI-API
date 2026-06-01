using Microsoft.EntityFrameworkCore;
using SecuroAPI.DataAccess.Entities;
using SecuroAPI.DataAccess.Repositories.Interfaces;

namespace SecuroAPI.DataAccess.Repositories;


public class APIRegistryRepository:BaseRepository<APIRegistry>, IAPIRegistryRepository
{
    private readonly SecuroAPIDbContext _dbContext;
    public APIRegistryRepository(SecuroAPIDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<APIRegistry>> GetAllById(string id)
    {
        return await _dbContext.APIRegistries
            .Where(r => r.UserID == id)
            .ToListAsync();
    }
}