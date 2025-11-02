using ec_project_api.Interfaces.PurchaseOrders;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;
using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.Dtos.response.purchaseorders;
using Microsoft.EntityFrameworkCore;

namespace ec_project_api.Services
{
    public class PurchaseOrderService 
    : BaseService<PurchaseOrder, int>, IPurchaseOrderService
    {
        private readonly IPurchaseOrderRepository _purchaseOrderRepo;
        private readonly DataContext _context;
        private readonly IStatusService _statusService;
        
        public PurchaseOrderService(
            IPurchaseOrderRepository purchaseOrderRepo, 
            DataContext context,
            IStatusService statusService)
        : base(purchaseOrderRepo)
        {
            _purchaseOrderRepo = purchaseOrderRepo;
            _context = context;
            _statusService = statusService;
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
                        options.OrderBy = q => q.OrderBy(po => po.PurchaseOrderItems.Sum(item => item.UnitPrice*item.Quantity));
                        break;
                    case "total_desc":
                        options.OrderBy = q => q.OrderByDescending(po => po.PurchaseOrderItems.Sum(item => item.UnitPrice*item.Quantity));
                        break;
                    default:
                        options.OrderBy = q => q.OrderByDescending(po => po.OrderDate);
                        break;
                }
            }

            options.PageNumber = pageNumber;
            options.PageSize = pageSize;
            options.Includes.Add(po => po.Status);
            options.Includes.Add(po => po.Supplier);
            options.Includes.Add(po => po.PurchaseOrderItems);
            var purchaseOrders = await base.GetAllAsync(options);
            foreach (var po in purchaseOrders)
            {
                po.TotalAmount = po.PurchaseOrderItems?.Sum(item => item.UnitPrice*item.Quantity) ?? 0;
            }
            return purchaseOrders;
        }


        public override async Task<PurchaseOrder?> GetByIdAsync(int id, QueryOptions<PurchaseOrder>? options = null)
        {
            options ??= new QueryOptions<PurchaseOrder>();
            options.Includes.Add(po => po.Status);
            options.Includes.Add(po => po.Supplier);
            options.Includes.Add(po => po.PurchaseOrderItems);
            return await base.GetByIdAsync(id, options);
        }

        // Helper methods to get status IDs
        private async Task<short> GetStatusIdByNameAsync(string statusName)
        {
            var status = await _statusService.FirstOrDefaultAsync(
                s => s.EntityType == EntityVariables.PurchaseOrder && s.Name == statusName
            );
            if (status == null)
                throw new InvalidOperationException($"Không tìm thấy trạng thái '{statusName}'.");
            return status.StatusId;
        }

        private async Task<string> GetStatusNameByIdAsync(short statusId)
        {
            var status = await _statusService.GetByIdAsync(statusId);
            return status?.Name ?? string.Empty;
        }

        public async Task<bool> UpdateStatusAsync(int id, short newStatusId)
        {
            var order = await _repository.GetByIdAsync(id);
            if (order == null)
                throw new InvalidOperationException(PurchaseOrderMessages.PurchaseOrderNotFound);

            var currentStatusName = await GetStatusNameByIdAsync(order.StatusId);
            var newStatusName = await GetStatusNameByIdAsync(newStatusId);

            // Get status IDs
            var draftId = await GetStatusIdByNameAsync(StatusVariables.Draft);
            var pendingId = await GetStatusIdByNameAsync(StatusVariables.Pending);
            var approvedId = await GetStatusIdByNameAsync(StatusVariables.Approved);
            var orderedId = await GetStatusIdByNameAsync(StatusVariables.Ordered);
            var receivedId = await GetStatusIdByNameAsync(StatusVariables.Received);
            var completedId = await GetStatusIdByNameAsync(StatusVariables.Completed);
            var cancelledId = await GetStatusIdByNameAsync(StatusVariables.Cancelled);

            // Validate status transitions
            switch (order.StatusId)
            {
                case var _ when order.StatusId == draftId:
                    // Draft -> Pending only
                    if (newStatusId != pendingId)
                        throw new InvalidOperationException("Đơn hàng Draft chỉ có thể chuyển sang Pending.");
                    break;

                case var _ when order.StatusId == pendingId:
                    // Pending -> Approved hoặc Cancelled
                    if (newStatusId != approvedId && newStatusId != cancelledId)
                        throw new InvalidOperationException("Đơn hàng Pending chỉ có thể chuyển sang Approved hoặc Cancelled.");
                    break;

                case var _ when order.StatusId == approvedId:
                    // Approved -> Ordered hoặc Cancelled
                    if (newStatusId != orderedId && newStatusId != cancelledId)
                        throw new InvalidOperationException("Đơn hàng Approved chỉ có thể chuyển sang Ordered hoặc Cancelled.");
                    break;

                case var _ when order.StatusId == orderedId:
                    // Ordered -> Received hoặc Cancelled
                    if (newStatusId != receivedId && newStatusId != cancelledId)
                        throw new InvalidOperationException("Đơn hàng Ordered chỉ có thể chuyển sang Received hoặc Cancelled.");
                    break;

                case var _ when order.StatusId == receivedId:
                    // Received -> Completed only
                    if (newStatusId != completedId)
                        throw new InvalidOperationException("Đơn hàng Received chỉ có thể chuyển sang Completed.");
                    break;

                case var _ when order.StatusId == completedId:
                    // Completed -> không thể chuyển sang trạng thái nào khác
                    throw new InvalidOperationException("Không thể thay đổi trạng thái đơn hàng đã Completed.");

                case var _ when order.StatusId == cancelledId:
                    // Cancelled -> không thể chuyển sang trạng thái nào khác
                    throw new InvalidOperationException(PurchaseOrderMessages.CannotModifyCancelled);

                default:
                    throw new InvalidOperationException(PurchaseOrderMessages.InvalidStatusTransition);
            }

            order.StatusId = newStatusId;
            order.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(order);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelAsync(int id)
        {
            var order = await _repository.GetByIdAsync(id);
            if (order == null)
                throw new InvalidOperationException(PurchaseOrderMessages.PurchaseOrderNotFound);

            var cancelledId = await GetStatusIdByNameAsync(StatusVariables.Cancelled);
            var completedId = await GetStatusIdByNameAsync(StatusVariables.Completed);
            var draftId = await GetStatusIdByNameAsync(StatusVariables.Draft);

            // Không thể hủy đơn đã Completed
            if (order.StatusId == completedId)
                throw new InvalidOperationException("Không thể hủy đơn hàng đã hoàn thành.");

            // Không thể hủy đơn đã bị hủy
            if (order.StatusId == cancelledId)
                throw new InvalidOperationException(PurchaseOrderMessages.CannotModifyCancelled);

            // Đơn Draft không cần hủy, có thể xóa trực tiếp
            if (order.StatusId == draftId)
                throw new InvalidOperationException("Đơn hàng Draft có thể xóa trực tiếp thay vì hủy.");

            order.StatusId = cancelledId;
            order.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(order);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<PurchaseOrderItem?> AddItemAsync(int poId, PurchaseOrderItem item)
        {
            if (item == null) return null;

            var po = await _purchaseOrderRepo.GetWithItemsAsync(poId);
            if (po == null)
                throw new InvalidOperationException(PurchaseOrderMessages.PurchaseOrderNotFound);

            var draftId = await GetStatusIdByNameAsync(StatusVariables.Draft);
            
            // Chỉ cho phép thêm item khi đơn hàng ở trạng thái Draft
            if (po.StatusId != draftId)
                throw new InvalidOperationException(PurchaseOrderMessages.CannotModifyProductsInPending);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                item.PurchaseOrderId = poId;
                po.PurchaseOrderItems.Add(item);
                po.UpdatedAt = DateTime.UtcNow;

                await _purchaseOrderRepo.UpdateAsync(po);
                await _purchaseOrderRepo.SaveChangesAsync();

                await transaction.CommitAsync();
                return item;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<PurchaseOrderItem?> UpdateItemAsync(int poId, int itemId, PurchaseOrderItem item)
        {
            if (item == null || item.Quantity < 1 || item.UnitPrice < 0 || item.ProfitPercentage < 0 || item.ProfitPercentage > 100)
                throw new InvalidOperationException("Dữ liệu item không hợp lệ.");

            var po = await _purchaseOrderRepo.GetWithItemsAsync(poId);
            if (po == null)
                throw new InvalidOperationException(PurchaseOrderMessages.PurchaseOrderNotFound);

            var draftId = await GetStatusIdByNameAsync(StatusVariables.Draft);
            
            // Chỉ cho phép cập nhật item khi đơn hàng ở trạng thái Draft
            if (po.StatusId != draftId)
                throw new InvalidOperationException(PurchaseOrderMessages.CannotModifyProductsInPending);

            var existingItem = po.PurchaseOrderItems.FirstOrDefault(i => i.PurchaseOrderItemId == itemId);
            if (existingItem == null)
                throw new InvalidOperationException("Item không tồn tại trong đơn hàng.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                existingItem.Quantity = item.Quantity;
                existingItem.UnitPrice = item.UnitPrice;
                existingItem.ProfitPercentage = item.ProfitPercentage;
                existingItem.UpdatedAt = DateTime.UtcNow;
                po.UpdatedAt = DateTime.UtcNow;

                await _purchaseOrderRepo.UpdateAsync(po);
                await _purchaseOrderRepo.SaveChangesAsync();

                await transaction.CommitAsync();
                return existingItem;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteItemAsync(int poId, int itemId)
        {
            var po = await _purchaseOrderRepo.GetWithItemsAsync(poId);
            if (po == null)
                throw new InvalidOperationException(PurchaseOrderMessages.PurchaseOrderNotFound);

            var draftId = await GetStatusIdByNameAsync(StatusVariables.Draft);
            
            // Chỉ cho phép xóa item khi đơn hàng ở trạng thái Draft
            if (po.StatusId != draftId)
                throw new InvalidOperationException(PurchaseOrderMessages.CannotModifyProductsInPending);

            var itemToRemove = po.PurchaseOrderItems.FirstOrDefault(i => i.PurchaseOrderItemId == itemId);
            if (itemToRemove == null)
                throw new InvalidOperationException("Item không tồn tại trong đơn hàng.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                po.PurchaseOrderItems.Remove(itemToRemove);
                po.UpdatedAt = DateTime.UtcNow;
                
                await _purchaseOrderRepo.UpdateAsync(po);
                await _purchaseOrderRepo.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        
        public async Task<PurchaseOrderStatisticsResponse> GetStatisticsAsync(
            DateTime? startDate = null, 
            DateTime? endDate = null, 
            int? supplierId = null)
        {
            // Get status IDs
            var draftId = await GetStatusIdByNameAsync(StatusVariables.Draft);
            var pendingId = await GetStatusIdByNameAsync(StatusVariables.Pending);
            var approvedId = await GetStatusIdByNameAsync(StatusVariables.Approved);
            var orderedId = await GetStatusIdByNameAsync(StatusVariables.Ordered);
            var receivedId = await GetStatusIdByNameAsync(StatusVariables.Received);
            var completedId = await GetStatusIdByNameAsync(StatusVariables.Completed);
            var cancelledId = await GetStatusIdByNameAsync(StatusVariables.Cancelled);

            // Build query
            var query = _context.Set<PurchaseOrder>()
                .Include(po => po.PurchaseOrderItems)
                .AsQueryable();

            // Apply filters
            if (startDate.HasValue)
                query = query.Where(po => po.OrderDate >= startDate.Value);
            
            if (endDate.HasValue)
                query = query.Where(po => po.OrderDate <= endDate.Value);
            
            if (supplierId.HasValue)
                query = query.Where(po => po.SupplierId == supplierId.Value);

            var orders = await query.ToListAsync();

            // Calculate statistics
            var statistics = new PurchaseOrderStatisticsResponse
            {
                TotalOrders = orders.Count,
                DraftOrders = orders.Count(o => o.StatusId == draftId),
                PendingOrders = orders.Count(o => o.StatusId == pendingId),
                ApprovedOrders = orders.Count(o => o.StatusId == approvedId),
                OrderedOrders = orders.Count(o => o.StatusId == orderedId),
                ReceivedOrders = orders.Count(o => o.StatusId == receivedId),
                CompletedOrders = orders.Count(o => o.StatusId == completedId),
                CancelledOrders = orders.Count(o => o.StatusId == cancelledId),
                TotalValue = orders.Sum(o => o.TotalAmount),
                TotalProducts = orders.Sum(o => o.PurchaseOrderItems?.Sum(item => item.Quantity) ?? 0)
            };

            return statistics;
        }
    }

}