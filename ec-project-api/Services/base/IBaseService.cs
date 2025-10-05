using System.Linq.Expressions;
using ec_project_api.Repository.Base;

namespace ec_project_api.Services.Bases {
    public interface IBaseService<TEntity, TKey> where TEntity : class {
        Task<IEnumerable<TEntity>> GetAllAsync(QueryOptions<TEntity>? options = null);
        Task<TEntity?> GetByIdAsync(TKey id, QueryOptions<TEntity>? options = null);
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity?> FirstOrDefaultAsync(
                    Expression<Func<TEntity, bool>> predicate,
                    QueryOptions<TEntity>? options = null);

        Task<bool> CreateAsync(TEntity entity);
        Task<bool> UpdateAsync(TEntity entity);
        Task<bool> UpdateRangeAsync(IEnumerable<TEntity> entities);
        Task<bool> DeleteAsync(TEntity entity);
        Task<bool> DeleteByIdAsync(TKey id);

        Task<int> SaveChangesAsync();
    }
}
