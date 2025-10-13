using ec_project_api.Models;
using ec_project_api.Services.Bases;

namespace ec_project_api.Interfaces.Suppliers
{
    public interface ISupplierService : IBaseService<Supplier, int>
    {
        Task<bool> UpdateStatusAsync(int id, short newStatusId);
    }
}
