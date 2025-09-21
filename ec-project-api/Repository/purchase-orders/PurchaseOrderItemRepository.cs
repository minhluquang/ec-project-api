using ec_project_api.Interfaces.PurchaseOrders;
using ec_project_api.Models;

public class PurchaseOrderItemRepository : Repository<PurchaseOrderItem>, IPurchaseOrderItemRepository
{
    public PurchaseOrderItemRepository(DataContext context) : base(context) { }
}
