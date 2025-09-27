using ec_project_api.Interfaces.Products;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services.product_images
{
    public class ProductImageService : BaseService<ProductImage, int>, IProductImageService
    {
        public ProductImageService(IProductImageRepository repository) : base(repository)
        {
        }

        public async Task<IEnumerable<ProductImage>> GetAllByProductIdAsync(int productId, QueryOptions<ProductImage>? options = null)
        {
            options ??= new QueryOptions<ProductImage>();

            options.Filter = pi => pi.ProductId == productId;
            var productImages = await GetAllAsync(options);

            return productImages;
        }
    }
}
