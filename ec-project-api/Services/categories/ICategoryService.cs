using ec_project_api.Models;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services {
    public interface ICategoryService : IBaseService<Category, short> {
        Task<IEnumerable<Category>> GetByParentIdsAsync(List<short> categoryIds);
        Task<IEnumerable<Category>> GetByParentIdAsync(short parentId);

    }
}