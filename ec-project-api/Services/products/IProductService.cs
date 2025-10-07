using ec_project_api.Models;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services {
    public interface IProductService : IBaseService<Product, int> {
        Task<bool> CreateAsync(Product product, ProductImage productImage, IFormFile FileImage);
        Task<IEnumerable<Product>> GetAllByCategoryidAsync(short categoryId, int? pageNumber = 1, int? pageSize = 12, decimal? minPrice = null, decimal? maxPrice = null, short? colorId = null, string? orderBy = null);
    }
}
