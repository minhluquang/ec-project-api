using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services {
    public interface IProductVariantService : IBaseService<ProductVariant, int> {
        Task<IEnumerable<ProductVariant>> GetAllByProductIdAsync(int productId, QueryOptions<ProductVariant>? options = null);
    }
}
