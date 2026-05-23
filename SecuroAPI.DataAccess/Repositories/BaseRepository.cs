using System.Linq.Expressions;
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

    public async Task<TEntity?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _dbContext.Set<TEntity>().FindAsync(id);
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

    public async Task DeleteAsync(Guid id)
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
    public async Task<bool> CheckExistsAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbContext.Set<TEntity>().AnyAsync(predicate);
    }

    public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbContext.Set<TEntity>().FirstOrDefaultAsync(predicate);
    }
}