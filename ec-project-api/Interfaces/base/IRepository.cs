using System.Linq.Expressions;
using ec_project_api.Repository.Base;

namespace ec_project_api.Interfaces
{
    public interface IRepository<TEntity, TKey> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync(QueryOptions<TEntity>? options = null);
        Task<TEntity?> GetByIdAsync(TKey id, QueryOptions<TEntity>? options = null);
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity?> FirstOrDefaultAsync(
                Expression<Func<TEntity, bool>> predicate,
                QueryOptions<TEntity>? options = null);

        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
        Task DeleteByIdAsync(TKey id);

        Task<int> SaveChangesAsync();
    }
}
