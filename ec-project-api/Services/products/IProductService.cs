using ec_project_api.Models;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services
{
    public interface IProductService : IBaseService<Product, int>
    {
        //Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
    }
}
