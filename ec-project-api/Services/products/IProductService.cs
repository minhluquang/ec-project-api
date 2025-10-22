using ec_project_api.Dtos.response.products;
using ec_project_api.Models;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services {
    public interface IProductService : IBaseService<Product, int> {
        Task<bool> CreateAsync(Product product, ProductImage productImage, IFormFile FileImage);
        Task<ProductFormMetaDto> GetProductFormMetaAsync();
        Task<ProductFilterOptionDto> GetFilterOptionsByCategorySlugAsync(string categorySlug);
        Task<(Product product, IEnumerable<Product> related)> GetBySlugAsync(string slug);
    }
}
