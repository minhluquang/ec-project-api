using System.Linq.Expressions;

namespace ec_project_api.Repository.Base
{
    public class QueryOptions<TEntity>
{
    public Expression<Func<TEntity, bool>>? Filter { get; set; }
    public Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? OrderBy { get; set; }
    public List<Expression<Func<TEntity, object>>> Includes { get; set; } = new();
    public List<string> IncludePaths { get; set; } = new(); 
    public Expression<Func<TEntity, object>>? Selector { get; set; }
    public bool AsNoTracking { get; set; } = true;

    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }

    public int? Skip => PageNumber.HasValue && PageSize.HasValue ? (PageNumber - 1) * PageSize : null;
    public int? Take => PageSize;
}

}
