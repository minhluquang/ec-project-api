using ec_project_api.Dtos.request.products;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services.product_images {
    public interface IProductImageService : IBaseService<ProductImage, int> {
        Task<IEnumerable<ProductImage>> GetAllByProductIdAsync(int productId, QueryOptions<ProductImage>? options = null);
        Task<bool> UploadSingleProductImageAsync(ProductImage productImage, IFormFile fileImage, bool findDisplayOrder = true);
    }
}