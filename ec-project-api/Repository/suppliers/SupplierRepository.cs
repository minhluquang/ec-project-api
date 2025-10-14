using ec_project_api.Interfaces.Suppliers;
using ec_project_api.Models;

public class SupplierRepository : Repository<Supplier, int>, ISupplierRepository
{
    public SupplierRepository(DataContext context) : base(context)
    {
    }
}
