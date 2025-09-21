using ec_project_api.Interfaces.Products;
using ec_project_api.Models;

public class MaterialRepository : Repository<Material>, IMaterialRepository
{
    public MaterialRepository(DataContext context) : base(context) { }
}
