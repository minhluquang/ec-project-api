using AutoMapper;
using ec_project_api.Dtos.request.purchaseorders;
using ec_project_api.Dtos.response.purchaseorders;
using ec_project_api.Models;
using ec_project_api.Services;
using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.Interfaces.Suppliers;
using ec_project_api.Dtos.response.pagination;
using ec_project_api.Repository.Base;

namespace ec_project_api.Facades.purchaseorders
{
    public class PurchaseOrderFacade
    {
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly IStatusService _statusService;
        private readonly ISupplierService _supplierService;
        private readonly IProductVariantService _productVariantService;
        private readonly IMapper _mapper;

        public PurchaseOrderFacade(
            IPurchaseOrderService purchaseOrderService,
            IStatusService statusService,
            ISupplierService supplierService,
            IProductVariantService productVariantService,
            IMapper mapper)
        {
            _purchaseOrderService = purchaseOrderService;
            _statusService = statusService;
            _supplierService = supplierService;
            _productVariantService = productVariantService;
            _mapper = mapper;
        }

        private async Task<short> GetStatusIdByNameAsync(string statusName)
        {
            var status = await _statusService.FirstOrDefaultAsync(
                s => s.EntityType == EntityVariables.PurchaseOrder && s.Name == statusName
            );
            if (status == null)
                throw new InvalidOperationException($"Không tìm thấy trạng thái '{statusName}'.");
            return status.StatusId;
        }
        public async Task<PagedResult<PurchaseOrderResponse>> GetAllPagedAsync(PurchaseOrderFilter filter)
        {
            var f = filter ?? new PurchaseOrderFilter();

            var options = new QueryOptions<PurchaseOrder>
            {
                PageNumber = f.PageNumber ?? 1,
                PageSize   = f.PageSize   ?? 10,
                Includes   = { p => p.Status, p => p.Supplier, p => p.PurchaseOrderItems }
            };

            DateTime? utcStart = f.StartDate?.ToUtcStartOfDay();
            DateTime? utcEnd   = f.EndDate?.ToUtcEndOfDay();

            options.Filter = po =>
                (!f.StatusId.HasValue    || po.StatusId == f.StatusId.Value) &&
                (!f.SupplierId.HasValue  || po.SupplierId == f.SupplierId.Value) &&
                (!utcStart.HasValue      || po.OrderDate >= utcStart.Value) &&
                (!utcEnd.HasValue        || po.OrderDate <= utcEnd.Value) &&
                (string.IsNullOrEmpty(f.Search) || (
                    po.PurchaseOrderId.ToString().Contains(f.Search) ||
                    (po.Supplier != null && po.Supplier.Name.Contains(f.Search))
                ));
            if (!string.IsNullOrEmpty(f.OrderBy))
            {
                options.OrderBy = f.OrderBy switch
                {
                    "orderDate_asc"   => q => q.OrderBy(po => po.OrderDate),
                    "orderDate_desc"  => q => q.OrderByDescending(po => po.OrderDate),
                    "createdAt_asc"   => q => q.OrderBy(po => po.CreatedAt),
                    "createdAt_desc"  => q => q.OrderByDescending(po => po.CreatedAt),
                    _ => q => q.OrderByDescending(po => po.CreatedAt) 
                };
            }

            var pagedResult = await _purchaseOrderService.GetAllPagedAsync(options);

            var dtoList = _mapper.Map<IEnumerable<PurchaseOrderResponse>>(pagedResult.Items);

            return new PagedResult<PurchaseOrderResponse>
            {
                Items      = dtoList,
                TotalCount = pagedResult.TotalCount,
                TotalPages = pagedResult.TotalPages,
                PageNumber = pagedResult.PageNumber,
                PageSize   = pagedResult.PageSize
            };
        }
        public async Task<PurchaseOrderResponse?> GetByIdAsync(int id)
        {
            var order = await _purchaseOrderService.GetByIdAsync(id);
            return _mapper.Map<PurchaseOrderResponse>(order);
        }

        public async Task<bool> CreateAsync(PurchaseOrderCreateRequest request)
        {
            var supplier = await _supplierService.GetByIdAsync(request.SupplierId);
            if (supplier == null)
                throw new InvalidOperationException(PurchaseOrderMessages.SupplierNotFound);

            var draftStatus = await _statusService.FirstOrDefaultAsync(
                s => s.EntityType == EntityVariables.PurchaseOrder && s.Name == StatusVariables.Draft
            );
            if (draftStatus == null)
                throw new InvalidOperationException(StatusMessages.StatusNotFound);

            foreach (var item in request.Items)
            {
                var variant = await _productVariantService.GetByIdAsync(item.ProductVariantId);
                if (variant == null)
                    throw new InvalidOperationException(
                        string.Format(PurchaseOrderMessages.ProductVariantNotFoundWithId, item.ProductVariantId));
            }

            var order = _mapper.Map<PurchaseOrder>(request);
            order.StatusId = draftStatus.StatusId;
            order.TotalAmount = request.Items.Sum(i => i.Quantity * i.UnitPrice);
            order.CreatedAt = DateTime.UtcNow;
            return await _purchaseOrderService.CreateAsync(order);
        }

        public async Task<bool> UpdateAsync(int id, PurchaseOrderUpdateRequest request)
        {
            var existing = await _purchaseOrderService.GetByIdAsync(id);
            if (existing == null)
                throw new InvalidOperationException(PurchaseOrderMessages.PurchaseOrderNotFound);

            var draftId = await GetStatusIdByNameAsync(StatusVariables.Draft);
            var pendingId = await GetStatusIdByNameAsync(StatusVariables.Pending);
            var approvedId = await GetStatusIdByNameAsync(StatusVariables.Approved);
            var orderedId = await GetStatusIdByNameAsync(StatusVariables.Ordered);
            var receivedId = await GetStatusIdByNameAsync(StatusVariables.Received);
            var completedId = await GetStatusIdByNameAsync(StatusVariables.Completed);
            var cancelledId = await GetStatusIdByNameAsync(StatusVariables.Cancelled);


            if (existing.StatusId == cancelledId)
                throw new InvalidOperationException(PurchaseOrderMessages.CannotModifyCancelled);


            if (existing.StatusId == approvedId || existing.StatusId == orderedId || 
                existing.StatusId == receivedId || existing.StatusId == completedId)
                throw new InvalidOperationException(PurchaseOrderMessages.CannotModifyAfterApproved);


            if (existing.StatusId == draftId)
            {
    
                if (request.SupplierId.HasValue)
                {
                    var supplier = await _supplierService.GetByIdAsync(request.SupplierId.Value);
                    if (supplier == null)
                        throw new InvalidOperationException(PurchaseOrderMessages.SupplierNotFound);
                    existing.SupplierId = request.SupplierId.Value;
                }

                if (request.OrderDate.HasValue)
                {
                    existing.OrderDate = request.OrderDate.Value;
                }

    
                var existingItems = existing.PurchaseOrderItems.ToList();

                if (request.Items == null)
                {
                    foreach (var oldItem in existingItems)
                    {
                        existing.PurchaseOrderItems.Remove(oldItem);
                    }
                }
                else
                {
        
                    foreach (var item in request.Items)
                    {
                        var variant = await _productVariantService.GetByIdAsync(item.ProductVariantId);
                        if (variant == null)
                            throw new InvalidOperationException(
                                string.Format(PurchaseOrderMessages.ProductVariantNotFoundWithId, item.ProductVariantId));
                    }

        
                    foreach (var oldItem in existingItems)
                    {
                        if (!request.Items.Any(i => i.ProductVariantId == oldItem.ProductVariantId))
                        {
                            existing.PurchaseOrderItems.Remove(oldItem);
                        }
                    }

        
                    foreach (var newItem in request.Items)
                    {
                        var existingItem = existing.PurchaseOrderItems.FirstOrDefault(i => i.ProductVariantId == newItem.ProductVariantId);
                        if (existingItem != null)
                        {
                            existingItem.Quantity = newItem.Quantity;
                            existingItem.UnitPrice = newItem.UnitPrice;
                            existingItem.ProfitPercentage = newItem.ProfitPercentage;
                            existingItem.IsPushed = newItem.IsPushed;
                            existingItem.UpdatedAt = DateTime.UtcNow;
                        }
                        else
                        {
                            existing.PurchaseOrderItems.Add(new PurchaseOrderItem
                            {
                                ProductVariantId = newItem.ProductVariantId,
                                Quantity = newItem.Quantity,
                                UnitPrice = newItem.UnitPrice,
                                ProfitPercentage = newItem.ProfitPercentage,
                                IsPushed = newItem.IsPushed,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            });
                        }
                    }
                }
            }

            else if (existing.StatusId == pendingId)
            {
                if (request.SupplierId.HasValue)
                {
                    var supplier = await _supplierService.GetByIdAsync(request.SupplierId.Value);
                    if (supplier == null)
                        throw new InvalidOperationException(PurchaseOrderMessages.SupplierNotFound);
                    existing.SupplierId = request.SupplierId.Value;
                }

                if (request.OrderDate.HasValue)
                {
                    existing.OrderDate = request.OrderDate.Value;
                }

    
                if (request.Items != null)
                {
                    throw new InvalidOperationException(PurchaseOrderMessages.CannotModifyProductsInPending);
                }
            }

            existing.UpdatedAt = DateTime.UtcNow;
            existing.TotalAmount = existing.PurchaseOrderItems.Sum(i => i.Quantity * i.UnitPrice);
            
            return await _purchaseOrderService.UpdateAsync(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _purchaseOrderService.GetByIdAsync(id);
            if (existing == null)
                throw new InvalidOperationException(PurchaseOrderMessages.PurchaseOrderNotFound);

            var draftId = await GetStatusIdByNameAsync(StatusVariables.Draft);
            var cancelledId = await GetStatusIdByNameAsync(StatusVariables.Cancelled);


            if (existing.StatusId != draftId)
            {
                if (existing.StatusId == cancelledId)
                    throw new InvalidOperationException(PurchaseOrderMessages.CannotModifyCancelled);
                else
                    throw new InvalidOperationException(PurchaseOrderMessages.CanOnlyCancelNotDelete);
            }

            return await _purchaseOrderService.DeleteAsync(existing);
        }

        public async Task<bool> UpdateStatusAsync(int id, short newStatusId)
        {
            var result = await _purchaseOrderService.UpdateStatusAsync(id, newStatusId);
            if (!result)
                throw new InvalidOperationException(PurchaseOrderMessages.PurchaseOrderUpdateFailed);

            return result;
        }

        public async Task<bool> CancelAsync(int id)
        {
            return await _purchaseOrderService.CancelAsync(id);
        }

        public async Task<PurchaseOrderItemResponse?> AddItemAsync(int poId, PurchaseOrderItemCreateRequest request)
        {
            var po = await _purchaseOrderService.GetByIdAsync(poId);
            if (po == null)
                throw new InvalidOperationException(PurchaseOrderMessages.PurchaseOrderNotFound);

            var variant = await _productVariantService.GetByIdAsync(request.ProductVariantId);
            if (variant == null)
                throw new InvalidOperationException(PurchaseOrderMessages.ProductVariantNotFound);

            var item = _mapper.Map<PurchaseOrderItem>(request);
            var createdItem = await _purchaseOrderService.AddItemAsync(poId, item);

            return _mapper.Map<PurchaseOrderItemResponse>(createdItem);
        }
        public async Task<PurchaseOrderItemResponse?> UpdateItemAsync(int poId, int itemId, PurchaseOrderItemCreateRequest request)
        {
            var po = await _purchaseOrderService.GetByIdAsync(poId);
            if (po == null)
                throw new InvalidOperationException(PurchaseOrderMessages.PurchaseOrderNotFound);

            var variant = await _productVariantService.GetByIdAsync(request.ProductVariantId);
            if (variant == null)
                throw new InvalidOperationException(PurchaseOrderMessages.ProductVariantNotFound);

            var item = _mapper.Map<PurchaseOrderItem>(request);
            var updatedItem = await _purchaseOrderService.UpdateItemAsync(poId, itemId, item);

            return _mapper.Map<PurchaseOrderItemResponse>(updatedItem);
        }

        public async Task<bool> DeleteItemAsync(int poId, int itemId)
        {
            var po = await _purchaseOrderService.GetByIdAsync(poId);
            if (po == null)
                throw new InvalidOperationException(PurchaseOrderMessages.PurchaseOrderNotFound);

            var result = await _purchaseOrderService.DeleteItemAsync(poId, itemId);
            if (!result)
                throw new InvalidOperationException(PurchaseOrderMessages.ItemDeleteFailed);

            return result;
        }

        public async Task<PurchaseOrderStatisticsResponse> GetStatisticsAsync(
            DateTime? startDate = null, 
            DateTime? endDate = null, 
            int? supplierId = null)
        {
            return await _purchaseOrderService.GetStatisticsAsync(startDate, endDate, supplierId);
        }
    }
}
