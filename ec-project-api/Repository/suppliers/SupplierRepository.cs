using ec_project_api.Interfaces.Suppliers;
using ec_project_api.Models;

public class SupplierRepository : Repository<Supplier, int>, ISupplierRepository
{
    private readonly DataContext _context;

    public SupplierRepository(DataContext context) : base(context)
    {
        _context = context;
    }
}
