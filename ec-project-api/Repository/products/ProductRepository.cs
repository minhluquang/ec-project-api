using ec_project_api.Interfaces.Products;
using ec_project_api.Models;

public class ProductRepository : Repository<Product,int>, IProductRepository
{
    public ProductRepository(DataContext context) : base(context) { }
}
