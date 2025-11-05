using ec_project_api.Interfaces.Products;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;
using Microsoft.EntityFrameworkCore;

namespace ec_project_api.Services {
    public class ProductVariantService : BaseService<ProductVariant, int>, IProductVariantService {
        public ProductVariantService(IProductVariantRepository repository) : base(repository) {
        }

        public override async Task<ProductVariant?> GetByIdAsync(int id, QueryOptions<ProductVariant>? options = null) {
            options ??= new QueryOptions<ProductVariant>();
            
            options.Includes.Add(pv => pv.Status!);
            options.Includes.Add(pv => pv.Size!);
            options.Includes.Add(pv => pv.Product!);
            
            return await base.GetByIdAsync(id, options);
        }

        public async Task<IEnumerable<ProductVariant>> GetAllByProductIdAsync(int productId, QueryOptions<ProductVariant>? options = null) {        
            options ??= new QueryOptions<ProductVariant>();

            options.Includes.Add(pv => pv.Size!);
            options.Includes.Add(pv => pv.Status!);
            options.Includes.Add(pv => pv.OrderItems!);
            options.IncludeThen.Add(q => q.Include(pv => pv.Product!).ThenInclude(p => p.Color!));
            options.Filter = pv => pv.ProductId == productId;

            var variants = await base.GetAllAsync(options);
            return variants;
        }
    }
}
