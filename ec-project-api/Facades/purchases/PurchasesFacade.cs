using AutoMapper;
using ec_project_api.Dtos.request.purchaseorders;
using ec_project_api.Dtos.response.purchaseorders;
using ec_project_api.Models;
using ec_project_api.Services;
using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.Interfaces.Suppliers;

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

        public async Task<IEnumerable<PurchaseOrderResponse>> GetAllAsync(Dtos.request.purchaseorders.PurchaseOrderFilter? filter = null)
        {
            var orders = await _purchaseOrderService.GetAllAsync(
                pageNumber: filter?.PageNumber,
                pageSize: filter?.PageSize,
                statusId: filter?.StatusId,
                supplierId: filter?.SupplierId,
                startDate: filter?.StartDate,
                endDate: filter?.EndDate,
                orderBy: filter?.OrderBy
            );

            return _mapper.Map<IEnumerable<PurchaseOrderResponse>>(orders);
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

            var pendingStatus = await _statusService.FirstOrDefaultAsync(
                s => s.EntityType == EntityVariables.PurchaseOrder && s.Name == StatusVariables.Pending
            );
            if (pendingStatus == null)
                throw new InvalidOperationException(StatusMessages.StatusNotFound);

            foreach (var item in request.Items)
            {
                var variant = await _productVariantService.GetByIdAsync(item.ProductVariantId);
                if (variant == null)
                    throw new InvalidOperationException(
                        string.Format(PurchaseOrderMessages.ProductVariantNotFoundWithId, item.ProductVariantId));
            }

            var order = _mapper.Map<PurchaseOrder>(request);
            order.StatusId = pendingStatus.StatusId;

            return await _purchaseOrderService.CreateAsync(order);
        }

        public async Task<bool> UpdateAsync(int id, PurchaseOrderUpdateRequest request)
        {
            var existing = await _purchaseOrderService.GetByIdAsync(id);
            if (existing == null)
                throw new InvalidOperationException(PurchaseOrderMessages.PurchaseOrderNotFound);

            if (request.SupplierId.HasValue)
            {
                var supplier = await _supplierService.GetByIdAsync(request.SupplierId.Value);
                if (supplier == null)
                    throw new InvalidOperationException(PurchaseOrderMessages.SupplierNotFound);
            }

            _mapper.Map(request, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            return await _purchaseOrderService.UpdateAsync(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _purchaseOrderService.GetByIdAsync(id);
            if (existing == null)
                throw new InvalidOperationException(PurchaseOrderMessages.PurchaseOrderNotFound);

            return await _purchaseOrderService.DeleteAsync(existing);
        }

        public async Task<bool> UpdateStatusAsync(int id, short newStatusId)
        {
            var result = await _purchaseOrderService.UpdateStatusAsync(id, newStatusId);
            if (!result)
                throw new InvalidOperationException(PurchaseOrderMessages.PurchaseOrderUpdateFailed);

            return result;
        }

        // Thêm item
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
    }
}
