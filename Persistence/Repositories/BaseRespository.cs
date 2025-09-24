
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using Persistence.Model;
using System.Linq.Expressions;

namespace Persistence.Repositories;

public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
{
    protected readonly DataContext _context;
    protected readonly DbSet<TEntity> _table;
    protected BaseRepository(DataContext context)
    {
        _context = context;
        _table = _context.Set<TEntity>();
    }

    public virtual async Task<RepositoryResult<IEnumerable<TEntity>>> GetAllAsync(Expression<Func<TEntity, bool>>? expression = null)
    {
        try
        {
            IQueryable<TEntity> query = _table; // This gives the ability to extend the query dynamically before executing, letting us add filters, etc. This is more efficient than fetching all data and then filtering in memory.

            if (expression != null)
                query = query.Where(expression);

            var entities = await query.ToListAsync();

            return new RepositoryResult<IEnumerable<TEntity>>
            {
                Success = true,
                Result = entities
            };
        }
        catch (Exception ex)
        {
            return new RepositoryResult<IEnumerable<TEntity>>
            {
                Success = false,
                Error = ex.Message
            };
        }
    }

    public virtual async Task<RepositoryResult<TEntity?>> GetAsync(Expression<Func<TEntity, bool>> expression)
    {
        try
        {
            var entity = await _table.FirstOrDefaultAsync(expression) ?? throw new Exception("Not found.");
            return new RepositoryResult<TEntity?> { Success = true, Result = entity };
        }
        catch (Exception ex)
        {
            return new RepositoryResult<TEntity?>
            {
                Success = false,
                Error = ex.Message
            };
        }
    }

    public virtual async Task<RepositoryResult> AlreadyExistsAsync(Expression<Func<TEntity, bool>> expression)
    {

        var result = await _table.AnyAsync(expression);
        return result
            ? new RepositoryResult { Success = true }
            : new RepositoryResult { Success = false, Error = "Entity not found." };
    }

    public virtual async Task<RepositoryResult<int>> CountAsync(Expression<Func<TEntity, bool>> expression)
    {
        try
        {
            var count = await _table.CountAsync(expression);
            return new RepositoryResult<int>
            {
                Success = true,
                Result = count
            };
        }
        catch (Exception ex)
        {
            return new RepositoryResult<int>
            {
                Success = false,
                Error = ex.Message
            };
        }
    }

    public virtual async Task<RepositoryResult> AddAsync(TEntity entity)
    {
        try
        {
            _table.Add(entity);
            await _context.SaveChangesAsync();

            return new RepositoryResult
            {
                Success = true
            };
        }
        catch (Exception ex)
        {
            return new RepositoryResult
            {
                Success = false,
                Error = ex.Message
            };
        }
    }

    public virtual async Task<RepositoryResult> UpdateAsync(TEntity entity)
    {
        try
        {
            _table.Update(entity);
            await _context.SaveChangesAsync();

            return new RepositoryResult
            {
                Success = true
            };
        }
        catch (Exception ex)
        {
            return new RepositoryResult
            {
                Success = false,
                Error = ex.Message
            };
        }
    }

    public virtual async Task<RepositoryResult> DeleteAsync(TEntity entity)
    {
        try
        {
            _table.Remove(entity);
            await _context.SaveChangesAsync();

            return new RepositoryResult
            {
                Success = true
            };
        }
        catch (Exception ex)
        {
            return new RepositoryResult
            {
                Success = false,
                Error = ex.Message
            };
        }
    }


    // New method to delete multiple entities with one call instead of N calls
    public virtual async Task<RepositoryResult> DeleteRangeAsync(IEnumerable<TEntity> entities) 
    {
        try
        {
            _table.RemoveRange(entities);
            await _context.SaveChangesAsync();

            return new RepositoryResult { Success = true };
        }
        catch (Exception ex)
        {
            return new RepositoryResult
            {
                Success = false,
                Error = ex.Message
            };
        }
    }

}
