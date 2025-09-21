using ec_project_api.Interfaces.Products;
using ec_project_api.Models;

public class ProductVariantRepository : Repository<ProductVariant>, IProductVariantRepository
{
    public ProductVariantRepository(DataContext context) : base(context) { }
}
