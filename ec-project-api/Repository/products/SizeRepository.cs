using ec_project_api.Interfaces.Products;
using ec_project_api.Models;

public class SizeRepository : Repository<Size, byte>, ISizeRepository
{
    public SizeRepository(DataContext context) : base(context) { }
}
