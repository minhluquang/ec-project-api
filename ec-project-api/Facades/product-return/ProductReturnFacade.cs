using ec_project_api.Constants.messages;
using ec_project_api.Constants.variables;
using ec_project_api.Dtos.request.product_return;
using ec_project_api.Dtos.response.productReturns;
using ec_project_api.Models;
using ec_project_api.Services;
using ec_project_api.Services.order_items;
using ec_project_api.Services.product_return;

namespace ec_project_api.Facades
{
    public class ProductReturnFacade
    {
        private readonly IProductReturnService _productReturnService;
        private readonly IOrderItemService _orderItemService;
        private readonly IProductVariantService _productVariantService;

        public ProductReturnFacade(
            IProductReturnService productReturnService,
            IOrderItemService orderItemService,
            IProductVariantService productVariantService
        )
        {
            _productReturnService = productReturnService;
            _orderItemService = orderItemService;
            _productVariantService = productVariantService;
        }

        public async Task<ProductReturnResponseDto> CreateProductReturnAsync(CreateProductReturnDto dto)
        {
            // Lấy sản phẩm muốn đổi trả
            var orderItem = await _orderItemService.GetByIdAsync(dto.OrderItemId);
            if (orderItem == null)
                throw new Exception(ProductReturnMessages.OrderItemNotFound);

            // 🔍 2. Nếu là đổi hàng (return_type = 1), bắt buộc có return_product_variant_id
            if (dto.ReturnType == 1 && dto.ReturnProductVariantId == null)
                throw new Exception(ProductReturnMessages.ExchangeRequiresReplacementProduct);

            // 🔍 3. Nếu là hoàn tiền (return_type = 2), bắt buộc có return_amount
            if (dto.ReturnType == 2)
                 dto.ReturnAmount = orderItem.Price;

            // 🧩 4. Tạo đối tượng ProductReturn
            var productReturn = new ProductReturn
            {
                OrderItemId = dto.OrderItemId,
                ReturnType = dto.ReturnType,
                ReturnReason = dto.ReturnReason,
                ReturnAmount = dto.ReturnAmount,
                ReturnProductVariantId = dto.ReturnProductVariantId,
                StatusId = dto.StatusId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _productReturnService.CreateAsync(productReturn);

            // 🔁 5. Xử lý tồn kho nếu cần
            var purchasedVariant = await _productVariantService.GetByIdAsync(orderItem.ProductVariantId);
            if (purchasedVariant != null)
            {
                // Nếu hoàn hàng hoặc đổi hàng, tăng lại stock của sản phẩm đã mua
                purchasedVariant.StockQuantity += 1;
                await _productVariantService.UpdateAsync(purchasedVariant);
                await _productVariantService.SaveChangesAsync();
            }

            if (dto.ReturnType == 1 && dto.ReturnProductVariantId.HasValue)
            {
                // Nếu đổi hàng, giảm stock của sản phẩm mới
                var newVariant = await _productVariantService.GetByIdAsync(dto.ReturnProductVariantId.Value);
                if (newVariant != null)
                {
                    if (newVariant.StockQuantity < 1)
                        throw new InvalidOperationException(ProductReturnMessages.ReplacementProductOutOfStock);

                    newVariant.StockQuantity -= 1;
                    await _productVariantService.UpdateAsync(newVariant);
                    await _productVariantService.SaveChangesAsync();
                }
            }

            // 💾 6. Lưu thay đổi
            await _productReturnService.SaveChangesAsync();
                                    
            // 🧾 7. Trả về DTO kết quả
            return new ProductReturnResponseDto
            {
                ReturnId = productReturn.ReturnId,
                OrderItemId = productReturn.OrderItemId,
                ReturnType = productReturn.ReturnType,
                ReturnReason = productReturn.ReturnReason,
                ReturnAmount = productReturn.ReturnAmount,
                StatusId = productReturn.StatusId,
                ReturnProductVariantId = productReturn.ReturnProductVariantId,
                CreatedAt = productReturn.CreatedAt
            };
        }
        public async Task<bool> DeleteProductReturnAsync(int returnId)
        {
            var productReturn = await _productReturnService.GetByIdAsync(returnId)
                ?? throw new KeyNotFoundException(ProductReturnMessages.ProductReturnNotFound);

            if (productReturn.Status.Name != StatusVariables.Draft)
                throw new InvalidOperationException(ProductReturnMessages.ProductReturnCannotBeDeleted);

            // 1️⃣ Lấy thông tin OrderItem
            var orderItem = await _orderItemService.GetByIdAsync(productReturn.OrderItemId);
            if (orderItem != null)
            {
                // 2️⃣ Hoàn trả tồn kho của sản phẩm đã mua (trừ lại vì đã tăng khi tạo return)
                var purchasedVariant = await _productVariantService.GetByIdAsync(orderItem.ProductVariantId);
                if (purchasedVariant != null)
                {
                    purchasedVariant.StockQuantity -= 1;
                    purchasedVariant.UpdatedAt = DateTime.UtcNow;
                    await _productVariantService.UpdateAsync(purchasedVariant);
                }
            }

            // 3️⃣ Nếu là đổi hàng, hoàn trả tồn kho của sản phẩm thay thế
            if (productReturn.ReturnType == 1 && productReturn.ReturnProductVariantId.HasValue)
            {
                var newVariant = await _productVariantService.GetByIdAsync(productReturn.ReturnProductVariantId.Value);
                if (newVariant != null)
                {
                    newVariant.StockQuantity += 1;
                    newVariant.UpdatedAt = DateTime.UtcNow;
                    await _productVariantService.UpdateAsync(newVariant);
                }
            }

            // 4️⃣ Lưu thay đổi tồn kho
            await _productVariantService.SaveChangesAsync();

            // 5️⃣ Xóa ProductReturn
            return await _productReturnService.DeleteAsync(productReturn);
        }
    }
}