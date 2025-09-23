using ec_project_api.Interfaces.Orders;
using ec_project_api.Models;

public class ProductReturnRepository : Repository<ProductReturn, int>, IProductReturnRepository
{
    public ProductReturnRepository(DataContext context) : base(context) { }
}
