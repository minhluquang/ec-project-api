using ec_project_api.Models;
using ec_project_api.Services.Bases;

namespace ec_project_api.Interfaces.Suppliers
{
    public interface ISupplierService : IBaseService<Supplier, int>
    {
        Task<IEnumerable<Supplier>> GetAllAsync(
                    bool isUserAdmin = false,
                    int? pageNumber = 1,
                    int? pageSize = 10,
                    int? statusId = null,
                    string? name = null,
                    string? orderBy = null);
        Task<bool> UpdateStatusAsync(int id, short newStatusId);
        Task<bool> DeleteAsync(Supplier s, short newStatusId);
    }
}
