using ec_project_api.Interfaces.PurchaseOrders;
using ec_project_api.Models;

public class PurchaseOrderRepository : Repository<PurchaseOrder>, IPurchaseOrderRepository
{
    public PurchaseOrderRepository(DataContext context) : base(context) { }
}
