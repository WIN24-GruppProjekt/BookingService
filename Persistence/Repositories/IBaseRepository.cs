using Persistence.Model;
using System.Linq.Expressions;

namespace Persistence.Repositories;

public interface IBaseRepository<TEntity> where TEntity : class
{
    Task<RepositoryResult> AddAsync(TEntity entity);
    Task<RepositoryResult> AlreadyExistsAsync(Expression<Func<TEntity, bool>> expression);
    Task<RepositoryResult> DeleteAsync(TEntity entity);
    Task<RepositoryResult> DeleteRangeAsync(IEnumerable<TEntity> entities); // New method to delete multiple entities with one call instead of N calls
    Task<RepositoryResult<IEnumerable<TEntity>>> GetAllAsync(Expression<Func<TEntity, bool>>? expression = null);
    Task<RepositoryResult<TEntity?>> GetAsync(Expression<Func<TEntity, bool>> expression);
    Task<RepositoryResult> UpdateAsync(TEntity entity);

}