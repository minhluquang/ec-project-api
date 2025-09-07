using ec_project_api.Models;

namespace ec_project_api.Interfaces
{
    public interface ICategoryRepository
    {
        ICollection<Category> GetCategories();
    }
}
