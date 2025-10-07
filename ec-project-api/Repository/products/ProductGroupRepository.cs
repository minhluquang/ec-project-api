using ec_project_api.Interfaces.Products;
using ec_project_api.Models;

public class ProductGroupRepository : Repository<ProductGroup, int>, IProductGroupRepository {
    public ProductGroupRepository(DataContext context) : base(context) { }
}