using ec_project_api.Models;

namespace ec_project_api.Interfaces.Suppliers
{
    public interface ISupplierRepository : IRepository<Supplier, int>
    {
        IQueryable<Supplier> Query();  // để filter bằng LINQ
    }
}
