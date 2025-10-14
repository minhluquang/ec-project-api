using ec_project_api.Models;
using ec_project_api.Services.Bases;
namespace ec_project_api.Services
{
    public interface IPurchaseOrderService : IBaseService<PurchaseOrder, int>
    {
        Task<bool> UpdateStatusAsync(int id, short newStatusId);                  
        Task<PurchaseOrderItem?> AddItemAsync(int poId, PurchaseOrderItem item);
        Task<PurchaseOrderItem?> UpdateItemAsync(int poId, int itemId, PurchaseOrderItem item);
        Task<bool> DeleteItemAsync(int poId, int itemId);
        Task<IEnumerable<PurchaseOrder>> GetAllAsync(int? pageNumber = 1,int? pageSize = 10,int? statusId = null,int? supplierId = null,DateTime? startDate = null,DateTime? endDate = null,string? orderBy = null);
    }

}