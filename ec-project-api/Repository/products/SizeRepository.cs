using ec_project_api.Interfaces.Products;
using ec_project_api.Models;

public class SizeRepository : Repository<Size>, ISizeRepository
{
    public SizeRepository(DataContext context) : base(context) { }
}
