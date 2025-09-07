using ec_project_api.Data;
using ec_project_api.Models;
using ec_project_api.Interfaces;

namespace ec_project_api.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DataContext _context;
        
        public CategoryRepository(DataContext context) 
        { 
            _context = context;
        }

        public ICollection<Category> GetCategories()
        {
            return _context.Categories.OrderBy(c => c.CategoryId).ToList();
        }
    }
}
