using ec_project_api.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly DataContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(DataContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    // Lấy tất cả
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    // Lấy theo ID
    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    // Tìm theo điều kiện (LINQ expression)
    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    // Thêm mới
    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    // Cập nhật
    public Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    // Xóa entity
    public Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    // Xóa theo Id
    public async Task DeleteByIdAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }

    // Lưu thay đổi
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
