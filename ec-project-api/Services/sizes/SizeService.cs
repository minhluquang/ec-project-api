using ec_project_api.Interfaces.Products;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services.sizes
{
    public class SizeService : BaseService<Size, byte>, ISizeService
    {
        public SizeService(ISizeRepository repository) : base(repository)
        {
        }

        public override async Task<Size?> GetByIdAsync(byte id, QueryOptions<Size>? options = null)
        {
            options ??= new QueryOptions<Size>();
            options.Includes.Add(s => s.Status);
            return await base.GetByIdAsync(id, options);
        }
    }
}