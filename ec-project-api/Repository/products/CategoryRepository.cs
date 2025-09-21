using ec_project_api.Interfaces.Products;
using ec_project_api.Models;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(DataContext context) : base(context) { }
}
