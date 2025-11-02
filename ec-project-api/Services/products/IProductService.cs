using ec_project_api.Dtos.response.products;
using ec_project_api.Models;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services {
    public interface IProductService : IBaseService<Product, int> {
        Task<bool> CreateAsync(Product product, ProductImage productImage, IFormFile FileImage);
        Task<ProductFormMetaDto> GetProductFormMetaAsync();
        Task<ProductFilterOptionDto> GetFilterOptionsByCategorySlugAsync(string? categorySlug, string? search);
        Task<(Product product, IEnumerable<Product> related)> GetBySlugAsync(string slug);
        Task<IEnumerable<Product>> SearchTop5Async(string search);
        Task<IEnumerable<Product>> GetTopByCategoryExcludingProductAsync(int categoryId, int excludeProductId, int top);
    }
}
