using Microsoft.EntityFrameworkCore;
using SecuroAPI.DataAccess.Repositories.Interfaces;

namespace SecuroAPI.DataAccess.Repositories;

public class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly SecuroAPIDbContext _dbContext;


    public BaseRepository(SecuroAPIDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        try
        {
            return await _dbContext.Set<TEntity>().ToListAsync();
        }
        catch (Exception e)
        {
            throw new Exception($"Error retrieving entities", e);
        }
    }

    public async Task<TEntity?> GetByIdAsync(int id)
    {
        try
        {
            return await _dbContext.Set<TEntity>().FindAsync();
        }
        catch (Exception e)
        {
            throw new Exception($"Error retrieving entity with ID {id}: {e.Message}", e);
        }
    }


    public async Task<TEntity> AddAsync(TEntity entity)
    {
        try
        {
            _dbContext.Set<TEntity>().Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }
        catch (Exception e)
        {
            throw new Exception($"Error adding entity", e);
        }
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        try
        {
            _dbContext.Set<TEntity>().Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }
        catch (Exception e)
        {
            throw new Exception($"Error updating entity", e);
        }
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            var entity = await _dbContext.Set<TEntity>().FindAsync(id);
            if (entity == null)
            {
                throw new Exception($"Entity with ID {id} was not found");
            }

            _dbContext.Set<TEntity>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            throw new Exception($"Error updating entity", e);
        }
    }
}