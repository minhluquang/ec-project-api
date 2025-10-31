using ec_project_api.Interfaces.Products;
using ec_project_api.Models;

public class ProductImageRepository : Repository<ProductImage, int>, IProductImageRepository
{
    public ProductImageRepository(DataContext context) : base(context) { }
}
