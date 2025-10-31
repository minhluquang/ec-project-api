using ec_project_api.Interfaces.Products;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;
using ec_project_api.Constants.variables;

namespace ec_project_api.Services.sizes
{
    public class SizeService : BaseService<Size, short>, ISizeService
    {
        public SizeService(ISizeRepository repository) : base(repository)
        {
        }
        
        public Task<IEnumerable<Size>> GetSizeOptionsAsync(QueryOptions<Size>? options = null)
        {
            options ??= new QueryOptions<Size>();
            options.Includes.Add(s => s.Status);
            options.Filter = s => s.Status!.Name == StatusVariables.Active;
            return base.GetAllAsync(options);
        }
        
        public override async Task<Size?> GetByIdAsync(short id, QueryOptions<Size>? options = null)
        {
            options ??= new QueryOptions<Size>();
            options.Includes.Add(s => s.Status);
            return await base.GetByIdAsync(id, options);
        }
    }
}