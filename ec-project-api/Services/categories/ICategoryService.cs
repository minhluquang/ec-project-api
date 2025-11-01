using ec_project_api.Models;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services
{
    public interface ICategoryService : IBaseService<Category, short>
    {
        Task<bool> CreateAsync(Category category, IFormFile? file = null);
        Task<bool> UpdateAsync(Category category, bool removeImage, IFormFile? file = null);
        Task<IEnumerable<Category>> GetByParentIdsAsync(List<short> categoryIds);
        Task<IEnumerable<Category>> GetByParentIdAsync(short parentId);
        Task<string> UploadAsync(Category category, IFormFile file);
        Task<bool> DeleteImageAsync(Category category);
    }
}
