using ec_project_api.Interfaces.Products;
using ec_project_api.Models;

public class SupplierRepository : Repository<Supplier>, ISupplierRepository
{
    public SupplierRepository(DataContext context) : base(context) { }
}
