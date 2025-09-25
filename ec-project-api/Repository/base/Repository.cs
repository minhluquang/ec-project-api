using ec_project_api.Interfaces;
using ec_project_api.Repository.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

public class Repository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class
{
    protected readonly DataContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public Repository(DataContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(QueryOptions<TEntity>? options = null)
    {
        IQueryable<TEntity> query = _dbSet;

        if (options?.Filter != null)
            query = query.Where(options.Filter);

        if (options?.Includes != null && options.Includes.Any())
        {
            foreach (var includeExpression in options.Includes)
            {
                query = query.Include(includeExpression);
            }
        }

        if (options?.OrderBy != null)
            query = options.OrderBy(query);

        if (options?.Skip.HasValue == true)
            query = query.Skip(options.Skip.Value);

        if (options?.Take.HasValue == true)
            query = query.Take(options.Take.Value);

        return await query.ToListAsync();
    }


    public async Task<TEntity?> GetByIdAsync(TKey id, QueryOptions<TEntity>? options = null)
    {
        IQueryable<TEntity> query = _dbSet;

        if (options?.Includes != null && options.Includes.Any())
        {
            foreach (var includeExpression in options.Includes)
            {
                query = query.Include(includeExpression);
            }
        }

        var keyName = _context.Model.FindEntityType(typeof(TEntity))!
            .FindPrimaryKey()!.Properties
            .Select(x => x.Name)
            .Single();

        return await query.FirstOrDefaultAsync(e => EF.Property<TKey>(e, keyName)!.Equals(id));
    }

    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public async Task<TEntity?> FirstOrDefaultAsync(
    Expression<Func<TEntity, bool>> predicate,
    QueryOptions<TEntity>? options = null)
    {
        IQueryable<TEntity> query = _dbSet;

        if (options?.Includes != null && options.Includes.Any())
        {
            foreach (var includeExpression in options.Includes)
            {
                query = query.Include(includeExpression);
            }
        }

        if (options?.AsNoTracking == true)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(predicate);
    }

    public async Task AddAsync(TEntity entity) => await _dbSet.AddAsync(entity);

    public async Task UpdateAsync(TEntity entity)
    {
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(TEntity entity)
    {
        _dbSet.Remove(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteByIdAsync(TKey id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
            _dbSet.Remove(entity);
    }

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
}
