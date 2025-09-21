using System.Linq.Expressions;

namespace ec_project_api.Interfaces
{
    public interface IRepository<T> where T : class
    {
        // Query
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        // Command
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task DeleteByIdAsync(int id);

        // Save
        Task<int> SaveChangesAsync();
    }
}
