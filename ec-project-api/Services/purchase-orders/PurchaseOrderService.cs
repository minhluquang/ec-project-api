using ec_project_api.Interfaces.PurchaseOrders;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services
{
    public class PurchaseOrderService 
    : BaseService<PurchaseOrder, int>, IPurchaseOrderService
    {
        private readonly IPurchaseOrderRepository _purchaseOrderRepo;
        private readonly DataContext _context;
        
        public PurchaseOrderService(IPurchaseOrderRepository purchaseOrderRepo, DataContext context)
        : base(purchaseOrderRepo)
        {
            _purchaseOrderRepo = purchaseOrderRepo;
            _context = context;
        }
       public async Task<IEnumerable<PurchaseOrder>> GetAllAsync(
            int? pageNumber = 1,
            int? pageSize = 10,
            int? statusId = null,
            int? supplierId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string? orderBy = null)
        {
            var options = new QueryOptions<PurchaseOrder>();

            // --- Bộ lọc ---
            options.Filter = po =>
                (!statusId.HasValue || po.StatusId == statusId.Value) &&
                (!supplierId.HasValue || po.SupplierId == supplierId.Value) &&
                (!startDate.HasValue || po.OrderDate >= startDate.Value) &&
                (!endDate.HasValue || po.OrderDate <= endDate.Value);

            // --- Sắp xếp ---
            if (!string.IsNullOrEmpty(orderBy))
            {
                switch (orderBy)
                {
                    case "date_asc":
                        options.OrderBy = q => q.OrderBy(po => po.OrderDate);
                        break;
                    case "date_desc":
                        options.OrderBy = q => q.OrderByDescending(po => po.OrderDate);
                        break;
                    case "total_asc":
                        options.OrderBy = q => q.OrderBy(po => po.TotalAmount);
                        break;
                    case "total_desc":
                        options.OrderBy = q => q.OrderByDescending(po => po.TotalAmount);
                        break;
                    default:
                        options.OrderBy = q => q.OrderByDescending(po => po.OrderDate); // mặc định: mới nhất trước
                        break;
                }
            }

            // --- Phân trang ---
            options.PageNumber = pageNumber;
            options.PageSize = pageSize;

            // --- Include ---
            options.Includes.Add(po => po.Status);
            options.Includes.Add(po => po.Supplier);
            options.Includes.Add(po => po.PurchaseOrderItems);

            // --- Gọi base ---
            return await base.GetAllAsync(options);
        }


        public override async Task<PurchaseOrder?> GetByIdAsync(int id, QueryOptions<PurchaseOrder>? options = null)
        {
            options ??= new QueryOptions<PurchaseOrder>();
            options.Includes.Add(po => po.Status);
            options.Includes.Add(po => po.Supplier);
            options.Includes.Add(po => po.PurchaseOrderItems);
            return await base.GetByIdAsync(id, options);
        }

        public async Task<bool> UpdateStatusAsync(int id, short newStatusId)
        {
            var order = await _repository.GetByIdAsync(id);
            if (order == null)
                throw new InvalidOperationException("Đơn hàng không tồn tại.");

            // Ràng buộc chuyển trạng thái hợp lệ
            switch (order.StatusId)
            {
                case 38: // Pending
                    if (newStatusId != 39 && newStatusId != 41)
                        throw new InvalidOperationException("Pending chỉ có thể chuyển sang Approved hoặc Cancelled.");
                    break;

                case 39: // Approved
                    if (newStatusId != 40 && newStatusId != 41)
                        throw new InvalidOperationException("Approved chỉ có thể chuyển sang Completed hoặc Cancelled.");
                    break;

                default:
                    throw new InvalidOperationException("Không thể thay đổi trạng thái ở giai đoạn này.");
            }

            order.StatusId = newStatusId;
            await _repository.UpdateAsync(order);
            return true;
        }


        public async Task<PurchaseOrderItem?> AddItemAsync(int poId, PurchaseOrderItem item)
        {
            if (item == null) return null;

            var po = await _purchaseOrderRepo.GetWithItemsAsync(poId);
            if (po == null || po.StatusId != 38) return null; // Chỉ cho phép thêm item khi PurchaseOrder ở trạng thái Pending (38)

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                item.PurchaseOrderId = poId; // Gán khóa ngoại
                po.PurchaseOrderItems.Add(item);

                await _purchaseOrderRepo.UpdateAsync(po);
                await _purchaseOrderRepo.SaveChangesAsync();

                await transaction.CommitAsync();
                return item;
            }
            catch
            {
                await transaction.RollbackAsync();
                return null;
            }
        }
        public async Task<PurchaseOrderItem?> UpdateItemAsync(int poId, int itemId, PurchaseOrderItem item)
        {
            if (item == null || item.Quantity < 1 || item.UnitPrice < 0 || item.ProfitPercentage < 0 || item.ProfitPercentage > 100)
                return null;

            var po = await _purchaseOrderRepo.GetWithItemsAsync(poId);
            if (po == null || po.StatusId != 38) return null;

            var existingItem = po.PurchaseOrderItems.FirstOrDefault(i => i.PurchaseOrderItemId == itemId);
            if (existingItem == null) return null;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                existingItem.Quantity = item.Quantity;
                existingItem.UnitPrice = item.UnitPrice;
                existingItem.ProfitPercentage = item.ProfitPercentage;
                existingItem.UpdatedAt = DateTime.UtcNow; 

                await _purchaseOrderRepo.UpdateAsync(po);
                await _purchaseOrderRepo.SaveChangesAsync();

                await transaction.CommitAsync();
                return existingItem;
            }
            catch
            {
                await transaction.RollbackAsync();
                return null;
            }
        }

        public async Task<bool> DeleteItemAsync(int poId, int itemId)
        {
            var po = await _purchaseOrderRepo.GetWithItemsAsync(poId);
            if (po == null || po.StatusId != 38) return false; // Chỉ cho phép xóa khi PurchaseOrder ở trạng thái Pending (38)

            var itemToRemove = po.PurchaseOrderItems.FirstOrDefault(i => i.PurchaseOrderItemId == itemId);
            if (itemToRemove == null) return false;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                po.PurchaseOrderItems.Remove(itemToRemove);
                await _purchaseOrderRepo.UpdateAsync(po);
                await _purchaseOrderRepo.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
    }

}