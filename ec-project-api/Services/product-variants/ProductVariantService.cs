using ec_project_api.Interfaces.Products;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services.product_variants
{
    public class ProductVariantService : BaseService<ProductVariant, int>, IProductVariantService
    {
        public ProductVariantService(IProductVariantRepository repository) : base(repository)
        {
        }

        public async Task<IEnumerable<ProductVariant>> GetAllByProductIdAsync(int productId, QueryOptions<ProductVariant>? options = null)
        {
            options ??= new QueryOptions<ProductVariant>();

            options.Includes.Add(pv => pv.Color);
            options.Includes.Add(pv => pv.Size);
            options.Filter = pv => pv.ProductId == productId;

            var variants = await base.GetAllAsync(options);
            return variants;
        }
    }
}
