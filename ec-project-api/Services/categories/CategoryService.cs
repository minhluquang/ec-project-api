using ec_project_api.Interfaces.Products;
using ec_project_api.Models;
using ec_project_api.Services.Bases;


namespace ec_project_api.Services.categories {
    public class CategoryService : BaseService<Category, short>, ICategoryService {
        public CategoryService(ICategoryRepository repository)
    : base(repository) {
        }
    }
}
