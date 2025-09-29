using ec_project_api.Models;
using ec_project_api.Services.Bases;
using System.Threading.Tasks;

namespace ec_project_api.Services {
    public interface IProductService : IBaseService<Product, int> {
        Task<bool> CreateAsync(Product product, ProductImage productImage, IFormFile FileImage);
    }
}
