using ec_project_api.Dtos.request.suppliers;
using ec_project_api.Dtos.response;
using ec_project_api.Models;
using ec_project_api.Services.Bases;

namespace ec_project_api.Interfaces.Suppliers
{
    public interface ISupplierService : IBaseService<Supplier, int>
    {
        Task<bool> UpdateStatusAsync(int id, int newStatusId);
        Task<IEnumerable<Supplier>> GetAllAsync(int? pageNumber = 1,int? pageSize = 10,int? statusId = null,string? name = null,string? orderBy = null);
    }
}
