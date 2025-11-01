using ec_project_api.Constants.messages;
using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.Dtos.request.product_return;
using ec_project_api.Dtos.response.orders;
using ec_project_api.Dtos.response.productReturns;
using ec_project_api.Models;
using ec_project_api.Services;
using ec_project_api.Services.order_items;
using ec_project_api.Services.orders;
using ec_project_api.Services.product_images;
using ec_project_api.Services.product_return;
using ec_project_api.Services.products;

namespace ec_project_api.Facades
{
    public class ProductReturnFacade
    {
        private readonly IProductReturnService _productReturnService;
        private readonly IOrderItemService _orderItemService;
        private readonly IProductVariantService _productVariantService;
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly IStatusService _statusService;
        private readonly IProductImageService _productImageService;
        public ProductReturnFacade(
            IProductReturnService productReturnService,
            IOrderItemService orderItemService,
            IProductVariantService productVariantService,
            IOrderService orderService,
            IUserService userService,
            IStatusService statusService ,
            IProductImageService productImageService
        )
        {
            _productReturnService = productReturnService;
            _orderItemService = orderItemService;
            _productVariantService = productVariantService;
            _orderService = orderService;
            _userService = userService;
            _statusService = statusService;
            _productImageService = productImageService;
        }

        public async Task<IEnumerable<ProductReturnResponseDto>> GetAllProductReturnsAsync()
        {
            var productReturns = await _productReturnService.GetAllAsync();
            var result = new List<ProductReturnResponseDto>();

            foreach (var pr in productReturns)
            {
                var orderItem = await _orderItemService.GetByIdAsync(pr.OrderItemId);
                var variant = await _productVariantService.GetByIdAsync(orderItem.ProductVariantId);
                var images = await _productImageService.GetByIdAsync(variant.ProductId);

                result.Add(new ProductReturnResponseDto
                {
                    ReturnId = pr.ReturnId,
                    OrderItemId = pr.OrderItemId,
                    ReturnType = pr.ReturnType,
                    ReturnReason = pr.ReturnReason,
                    ReturnAmount = pr.ReturnAmount,
                    ReturnProductVariantId = pr.ReturnProductVariantId,
                    StatusId = pr.StatusId,
                    StatusName = pr.Status?.Name,
                    ProductName = variant.Product?.Name,
                    ProductImageUrl = images.ImageUrl,
                    CreatedAt = pr.CreatedAt,
                    OrderDto = new OrderDto
                    {
                        OrderId = pr.OrderItem?.OrderId ?? 0,
                        TotalAmount = pr.OrderItem?.Order?.TotalAmount ?? 0,
                        CreatedAt = pr.OrderItem?.Order?.CreatedAt ?? DateTime.MinValue,
                        AddressInfo = pr.OrderItem?.Order?.AddressInfo,
                        IsFreeShip = pr.OrderItem?.Order?.IsFreeShip ?? false,
                        ShippingFee = pr.OrderItem?.Order?.ShippingFee ?? 0,
                        UserId = pr.OrderItem?.Order?.UserId ?? 0
                    },
                    UserOrderDto = new UserOrderDto
                    {
                        UserId = pr.OrderItem?.Order?.User?.UserId ?? 0,
                        FullName = pr.OrderItem?.Order?.User?.FullName
                    },
                });
            }

            return result;
        }



        public async Task<ProductReturnResponseDto> CreateProductReturnAsync(CreateProductReturnDto dto)
        {
            // Lấy sản phẩm muốn đổi trả
            var orderItem = await _orderItemService.GetByIdAsync(dto.OrderItemId);
            if (orderItem == null)
                throw new Exception(ProductReturnMessages.OrderItemNotFound);

            var order = await _orderService.GetByIdAsync(orderItem.OrderId) ?? 
                throw new Exception(OrderMessages.OrderNotFound);

            var user = await _userService.GetByIdAsync(order.UserId) ?? 
                throw new Exception(UserMessages.UserNotFound);

            var statusDraft = await _statusService.FirstOrDefaultAsync(
                s => s.EntityType == EntityVariables.ProductReturn && s.Name == StatusVariables.Draft) ?? 
                throw new Exception(StatusMessages.StatusNotFound);


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
                StatusId = statusDraft.StatusId,
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
                StatusId = statusDraft.StatusId,
                ReturnProductVariantId = productReturn.ReturnProductVariantId,
                CreatedAt = productReturn.CreatedAt,
                
                OrderDto = new OrderDto
                {
                    OrderId = order.OrderId,
                    TotalAmount = order.TotalAmount,
                    CreatedAt = order.CreatedAt,
                    AddressInfo = order.AddressInfo,
                    IsFreeShip = order.IsFreeShip,
                    ShippingFee = order.ShippingFee,
                    UserId = order.UserId
                },
                UserOrderDto = new UserOrderDto
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                }
            };
        }
        public async Task<bool> ApproveProductReturnAsync(int returnId)
        {
            var productReturn = await _productReturnService.GetByIdAsync(returnId)
                ?? throw new KeyNotFoundException(ProductReturnMessages.ProductReturnNotFound);
            var approvedStatus = await _statusService.FirstOrDefaultAsync(
                s => s.EntityType == EntityVariables.ProductReturn && s.Name == StatusVariables.Approved)
                ?? throw new Exception(StatusMessages.StatusNotFound);
            productReturn.StatusId = approvedStatus.StatusId;
            productReturn.UpdatedAt = DateTime.UtcNow;
            return await _productReturnService.UpdateAsync(productReturn);
        }
        public async Task<bool> RejectedProductReturnAsync(int returnId)
        {
            var productReturn = await _productReturnService.GetByIdAsync(returnId)
                ?? throw new KeyNotFoundException(ProductReturnMessages.ProductReturnNotFound);
            var cancelledStatus = await _statusService.FirstOrDefaultAsync(
                s => s.EntityType == EntityVariables.ProductReturn && s.Name == StatusVariables.Rejected)
                ?? throw new Exception(StatusMessages.StatusNotFound);
            productReturn.StatusId = cancelledStatus.StatusId;
            productReturn.UpdatedAt = DateTime.UtcNow;
            return await _productReturnService.UpdateAsync(productReturn);
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