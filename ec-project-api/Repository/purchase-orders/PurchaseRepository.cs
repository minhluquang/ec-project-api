using ec_project_api.Interfaces.PurchaseOrders;
using ec_project_api.Models;
using Microsoft.EntityFrameworkCore;

public class PurchaseOrderRepository 
    : Repository<PurchaseOrder, int>, IPurchaseOrderRepository
{
    public PurchaseOrderRepository(DataContext context) : base(context) { }

    public async Task<PurchaseOrder?> GetWithItemsAsync(int id)
    {
        return await _dbSet
            .Include(po => po.PurchaseOrderItems)
            .FirstOrDefaultAsync(po => po.PurchaseOrderId == id);
    }
}
