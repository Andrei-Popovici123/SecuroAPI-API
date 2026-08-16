using System.Linq.Expressions;

namespace SecuroAPI.DataAccess.Repositories.Interfaces;

public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(Guid id);
    Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null);
    Task<TEntity> AddAsync(TEntity entity);
    Task<TEntity> UpdateAsync(TEntity entity);
    Task DeleteAsync(Guid id);
    //To add a way to check generic existence in the base repository
    Task<TEntity?> GetAsync(Expression<Func<TEntity,bool>> predicate);
    Task<bool> CheckExistsAsync(Expression<Func<TEntity, bool>> predicate);
}