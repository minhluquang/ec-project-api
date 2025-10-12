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

        // Lấy toàn bộ đơn nhập
        public async Task<IEnumerable<PurchaseOrderResponse>> GetAllAsync()
        {
            var orders = await _purchaseOrderService.GetAllAsync();
            return _mapper.Map<IEnumerable<PurchaseOrderResponse>>(orders);
        }

        // Lấy đơn nhập theo Id
        public async Task<PurchaseOrderResponse?> GetByIdAsync(int id)
        {
            var order = await _purchaseOrderService.GetByIdAsync(id);
            return _mapper.Map<PurchaseOrderResponse>(order);
        }

        // Tạo mới đơn nhập hàng
        public async Task<bool> CreateAsync(PurchaseOrderCreateRequest request)
        {
            var supplier = await _supplierService.GetByIdAsync(request.SupplierId);
            if (supplier == null)
                throw new InvalidOperationException("Nhà cung cấp không tồn tại.");

            var pendingStatus = await _statusService.FirstOrDefaultAsync(
                s => s.EntityType == EntityVariables.PurchaseOrder && s.Name == StatusVariables.Pending
            );
            if (pendingStatus == null)
                throw new InvalidOperationException(StatusMessages.StatusNotFound);

            foreach (var item in request.Items)
            {
                var variant = await _productVariantService.GetByIdAsync(item.ProductVariantId);
                if (variant == null)
                    throw new InvalidOperationException($"Biến thể sản phẩm (ID: {item.ProductVariantId}) không tồn tại.");
            }

            var order = _mapper.Map<PurchaseOrder>(request);
            order.StatusId = pendingStatus.StatusId;

            // 5️⃣ Lưu vào database
            return await _purchaseOrderService.CreateAsync(order);
        }

        // Cập nhật thông tin đơn nhập
        public async Task<bool> UpdateAsync(int id, PurchaseOrderUpdateRequest request)
        {
            var existing = await _purchaseOrderService.GetByIdAsync(id);
            if (existing == null)
                throw new InvalidOperationException("Đơn nhập hàng không tồn tại.");

            if (request.SupplierId.HasValue)
            {
                var supplier = await _supplierService.GetByIdAsync(request.SupplierId.Value);
                if (supplier == null)
                    throw new InvalidOperationException("Nhà cung cấp không tồn tại.");
            }

            _mapper.Map(request, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            return await _purchaseOrderService.UpdateAsync(existing);
        }

        // Xóa đơn nhập hàng
        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _purchaseOrderService.GetByIdAsync(id);
            if (existing == null)
                throw new InvalidOperationException("Đơn nhập hàng không tồn tại.");

            return await _purchaseOrderService.DeleteAsync(existing);
        }
        // Cập nhật trạng thái đơn nhập
        public async Task<bool> UpdateStatusAsync(int id, short newStatusId)
        {
            var result = await _purchaseOrderService.UpdateStatusAsync(id, newStatusId);
            if (!result)
                throw new InvalidOperationException("Không thể cập nhật trạng thái đơn nhập hàng.");
            return result;
        }
        public async Task<PurchaseOrderItemResponse?> AddItemAsync(int poId, PurchaseOrderItemCreateRequest request)
        {
            var po = await _purchaseOrderService.GetByIdAsync(poId);
            if (po == null)
                throw new InvalidOperationException("Đơn nhập hàng không tồn tại.");

            var variant = await _productVariantService.GetByIdAsync(request.ProductVariantId);
            if (variant == null)
                throw new InvalidOperationException("Biến thể sản phẩm không tồn tại.");

            var item = _mapper.Map<PurchaseOrderItem>(request);
            var createdItem = await _purchaseOrderService.AddItemAsync(poId, item);

            return _mapper.Map<PurchaseOrderItemResponse>(createdItem);
        }

        // Cập nhật item trong đơn nhập hàng
        public async Task<PurchaseOrderItemResponse?> UpdateItemAsync(int poId, int itemId, PurchaseOrderItemCreateRequest request)
        {
            var po = await _purchaseOrderService.GetByIdAsync(poId);
            if (po == null)
                throw new InvalidOperationException("Đơn nhập hàng không tồn tại.");

            var variant = await _productVariantService.GetByIdAsync(request.ProductVariantId);
            if (variant == null)
                throw new InvalidOperationException("Biến thể sản phẩm không tồn tại.");

            var item = _mapper.Map<PurchaseOrderItem>(request);
            var updatedItem = await _purchaseOrderService.UpdateItemAsync(poId, itemId, item);

            return _mapper.Map<PurchaseOrderItemResponse>(updatedItem);
        }

        // Xóa item trong đơn nhập hàng
        public async Task<bool> DeleteItemAsync(int poId, int itemId)
        {
            var po = await _purchaseOrderService.GetByIdAsync(poId);
            if (po == null)
                throw new InvalidOperationException("Đơn nhập hàng không tồn tại.");

            var result = await _purchaseOrderService.DeleteItemAsync(poId, itemId);
            if (!result)
                throw new InvalidOperationException("Không thể xóa item trong đơn nhập hàng.");

            return result;
        }
    }
}
