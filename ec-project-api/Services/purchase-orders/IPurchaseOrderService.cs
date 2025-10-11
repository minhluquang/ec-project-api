using ec_project_api.Models;
using ec_project_api.Services.Bases;
namespace ec_project_api.Services
{
    public interface IPurchaseOrderService : IBaseService<PurchaseOrder, int>
    {
        Task<bool> UpdateStatusAsync(int id, int newStatusId);                  
        Task<PurchaseOrderItem?> AddItemAsync(int poId, PurchaseOrderItem item);
        Task<PurchaseOrderItem?> UpdateItemAsync(int poId, int itemId, PurchaseOrderItem item);
        Task<bool> DeleteItemAsync(int poId, int itemId);
    }

}