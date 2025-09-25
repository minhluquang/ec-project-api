using System.Linq.Expressions;
using ec_project_api.Interfaces;
using ec_project_api.Repository.Base;

namespace ec_project_api.Services.Bases
{
    public abstract class BaseService<TEntity, TKey> : IBaseService<TEntity, TKey> 
        where TEntity : class
    {
        protected readonly IRepository<TEntity, TKey> _repository;

        protected BaseService(IRepository<TEntity, TKey> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(QueryOptions<TEntity>? options = null)
        {
            return await _repository.GetAllAsync(options);
        }

        public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _repository.FindAsync(predicate);
        }

        public virtual async Task<TEntity?> GetByIdAsync(TKey id, QueryOptions<TEntity>? options = null)
        {
            return await _repository.GetByIdAsync(id, options);
        }

        public virtual async Task<TEntity?> FirstOrDefaultAsync(
            Expression<Func<TEntity, bool>> predicate,
            QueryOptions<TEntity>? options = null)
        {
            return await _repository.FirstOrDefaultAsync(predicate, options);
        }

        public virtual async Task<bool> CreateAsync(TEntity entity)
        {
            await _repository.AddAsync(entity);
            return await _repository.SaveChangesAsync() > 0;
        }

        public virtual async Task<bool> UpdateAsync(TEntity entity)
        {
            await _repository.UpdateAsync(entity);
            return await _repository.SaveChangesAsync() > 0;
        }

        public virtual async Task<bool> DeleteAsync(TEntity entity)
        {
            if (entity == null) return false;
            await _repository.DeleteAsync(entity);
            return await _repository.SaveChangesAsync() > 0;
        }

        public virtual async Task<bool> DeleteByIdAsync(TKey id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;
            await _repository.DeleteAsync(entity);
            return await _repository.SaveChangesAsync() > 0;
        }

        public virtual Task<int> SaveChangesAsync()
        {
            return _repository.SaveChangesAsync();
        }
    }
}