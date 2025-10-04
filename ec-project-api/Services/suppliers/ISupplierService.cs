using ec_project_api.Dtos.request.suppliers;
using ec_project_api.Dtos.response;
using ec_project_api.Models;
using ec_project_api.Services.Bases;

namespace ec_project_api.Interfaces.Suppliers
{
    public interface ISupplierService : IBaseService<Supplier, int>
    {
        Task<PagedResponse<Supplier>> GetPagedAsync(SupplierQueryRequest filter);
    }
}
