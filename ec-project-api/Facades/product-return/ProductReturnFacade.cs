using ec_project_api.Constants.messages;
using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.Dtos.request.product_return;
using ec_project_api.Dtos.response.orders;
using ec_project_api.Dtos.response.pagination;
using ec_project_api.Dtos.response.productReturns;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services;
using ec_project_api.Services.order_items;
using ec_project_api.Services.orders;
using ec_project_api.Services.product_images;
using ec_project_api.Services.product_return;
using ec_project_api.Services.products;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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
            IStatusService statusService,
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

        /// <summary>
        /// Lấy danh sách ProductReturn có phân trang
        /// </summary>
        public async Task<PagedResult<ProductReturnResponseDto>> GetAllPagedAsync(ProductReturnFilter filter)
        {
            var options = new QueryOptions<ProductReturn>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Filter = BuildProductReturnFilter(filter),
                OrderBy = q => q.OrderByDescending(pr => pr.CreatedAt)
            };

            // Include relationships
            options.Includes.Add(pr => pr.OrderItem);
            options.Includes.Add(pr => pr.Status);
            options.Includes.Add(pr => pr.ReturnProductVariant);

            // Deep includes
            options.IncludeThen.Add(q => q
                .Include(pr => pr.OrderItem)
                    .ThenInclude(oi => oi.Order)
                        .ThenInclude(o => o.User));

            options.IncludeThen.Add(q => q
                .Include(pr => pr.OrderItem)
                    .ThenInclude(oi => oi.ProductVariant)
                        .ThenInclude(pv => pv.Product)
                            .ThenInclude(p => p.ProductImages));
            

            var pagedResult = await _productReturnService.GetAllPagedAsync(options);

            var dtoList = new List<ProductReturnResponseDto>();
            foreach (var pr in pagedResult.Items)
            {
                var variant = pr.OrderItem?.ProductVariant;
                var product = variant?.Product;
                var images = product?.ProductImages?.FirstOrDefault();

                dtoList.Add(new ProductReturnResponseDto
                {
                    ReturnId = pr.ReturnId,
                    OrderItemId = pr.OrderItemId,
                    ReturnType = pr.ReturnType,
                    ReturnReason = pr.ReturnReason,
                    ReturnAmount = pr.ReturnAmount,
                    ReturnProductVariantId = pr.ReturnProductVariantId,
                    StatusId = pr.StatusId,
                    StatusName = pr.Status?.Name,
                    ProductName = product?.Name,
                    ProductImageUrl = images?.ImageUrl,
                    CreatedAt = pr.CreatedAt,
                    OrderDto = new OrderDto
                    {
                        OrderId = pr.OrderItem?.Order?.OrderId ?? 0,
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
                    }
                });
            }

            return new PagedResult<ProductReturnResponseDto>
            {
                Items = dtoList,
                TotalCount = pagedResult.TotalCount,
                TotalPages = pagedResult.TotalPages,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        /// <summary>
        /// Build filter expression for ProductReturn
        /// </summary>
        private static Expression<Func<ProductReturn, bool>> BuildProductReturnFilter(ProductReturnFilter filter)
        {
            return pr =>
                (string.IsNullOrEmpty(filter.StatusName) ||
                    (pr.Status != null && pr.Status.Name == filter.StatusName && 
                     pr.Status.EntityType == EntityVariables.ProductReturn)) &&
                (string.IsNullOrEmpty(filter.Search) ||
                    pr.ReturnId.ToString().Contains(filter.Search) ||
                    (pr.OrderItem != null && pr.OrderItem.Order != null && 
                     pr.OrderItem.Order.User != null && 
                     pr.OrderItem.Order.User.FullName.Contains(filter.Search))) &&
                (!filter.ReturnType.HasValue || pr.ReturnType == filter.ReturnType.Value);
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
            var orderItem = await _orderItemService.GetByIdAsync(dto.OrderItemId);
            if (orderItem == null)
                throw new Exception(ProductReturnMessages.OrderItemNotFound);

            var order = await _orderService.GetByIdAsync(orderItem.OrderId) ?? 
                throw new Exception(OrderMessages.OrderNotFound);

            var user = await _userService.GetByIdAsync(order.UserId) ?? 
                throw new Exception(UserMessages.UserNotFound);

            var existingReturn = await _productReturnService.FirstOrDefaultAsync(
                pr => pr.OrderItemId == dto.OrderItemId);

            if(existingReturn != null)
                throw new Exception(ProductReturnMessages.ProductReturnAlreadyExistsForOrderItem);

            var statusInit = await _statusService.FirstOrDefaultAsync(
                s => s.EntityType == EntityVariables.ProductReturn && s.Name == StatusVariables.Pending) ?? 
                throw new Exception(StatusMessages.StatusNotFound);

            var productReturn = new ProductReturn();

            if (dto.ReturnType == 2)
                productReturn.ReturnProductVariantId = orderItem.ProductVariantId;

            if (dto.ReturnType == 1)
                productReturn.ReturnAmount = orderItem.Price;

            productReturn.OrderItemId = dto.OrderItemId;
            productReturn.ReturnType = dto.ReturnType;
            productReturn.ReturnReason = dto.ReturnReason;
            productReturn.StatusId = statusInit.StatusId;
            productReturn.CreatedAt = DateTime.UtcNow;
            productReturn.UpdatedAt = DateTime.UtcNow;

            await _productReturnService.CreateAsync(productReturn);
            await _productReturnService.SaveChangesAsync();

            return new ProductReturnResponseDto
            {
                ReturnId = productReturn.ReturnId,
                OrderItemId = productReturn.OrderItemId,
                ReturnType = productReturn.ReturnType,
                ReturnReason = productReturn.ReturnReason,
                ReturnAmount = productReturn.ReturnAmount,
                StatusId = statusInit.StatusId,
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

        public async Task<bool> CompleteProductReturnAsync(int returnId)
        {
            var productReturn = await _productReturnService.GetByIdAsync(returnId)
                ?? throw new KeyNotFoundException(ProductReturnMessages.ProductReturnNotFound);
            var completedStatus = await _statusService.FirstOrDefaultAsync(
                s => s.EntityType == EntityVariables.ProductReturn && s.Name == StatusVariables.Completed)
                ?? throw new Exception(StatusMessages.StatusNotFound);
            productReturn.StatusId = completedStatus.StatusId;
            productReturn.UpdatedAt = DateTime.UtcNow;
            return await _productReturnService.UpdateAsync(productReturn);
        }
        public async Task<bool> CompleteProductReturnForReturnAsync(int returnId)
        {
            var productReturn = await _productReturnService.GetByIdAsync(returnId)
                ?? throw new KeyNotFoundException(ProductReturnMessages.ProductReturnNotFound);
           var orderItem = await _orderItemService.GetByIdAsync(productReturn.OrderItemId);
            if (orderItem != null)
            {
                var purchasedVariant = await _productVariantService.GetByIdAsync(orderItem.ProductVariantId);
                if (purchasedVariant != null)
                {
                    purchasedVariant.StockQuantity += 1;
                    purchasedVariant.UpdatedAt = DateTime.UtcNow;
                    await _productVariantService.UpdateAsync(purchasedVariant);
                }
            }
            await _productVariantService.SaveChangesAsync();
           
            return await CompleteProductReturnAsync(returnId);
        }
        public async Task<bool> CompleteProductReturnForExchangeAsync(int returnId)
        {
            var productReturn = await _productReturnService.GetByIdAsync(returnId)
                ?? throw new KeyNotFoundException(ProductReturnMessages.ProductReturnNotFound);
            var orderItem = await _orderItemService.GetByIdAsync(productReturn.OrderItemId);
            if (orderItem != null && productReturn.ReturnProductVariantId.HasValue)
            {
                var newVariant = await _productVariantService.GetByIdAsync(orderItem.ProductVariantId);
                if (newVariant != null)
                {
                    newVariant.StockQuantity -= 1;
                    newVariant.UpdatedAt = DateTime.UtcNow;
                    await _productVariantService.UpdateAsync(newVariant);
                }
            }
            await _productVariantService.SaveChangesAsync();
            return await CompleteProductReturnAsync(returnId);
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

            var orderItem = await _orderItemService.GetByIdAsync(productReturn.OrderItemId);
            if (orderItem != null)
            {
                var purchasedVariant = await _productVariantService.GetByIdAsync(orderItem.ProductVariantId);
                if (purchasedVariant != null)
                {
                    purchasedVariant.StockQuantity -= 1;
                    purchasedVariant.UpdatedAt = DateTime.UtcNow;
                    await _productVariantService.UpdateAsync(purchasedVariant);
                }
            }

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

            await _productVariantService.SaveChangesAsync();
            return await _productReturnService.DeleteAsync(productReturn);
        }
    }
}
