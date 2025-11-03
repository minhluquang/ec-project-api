using AutoMapper;
using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.Dtos.response.pagination;
using ec_project_api.Dtos.request.orders;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.orders;
using ec_project_api.Interfaces.Ships;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services;
using ec_project_api.Services.discounts;
using ec_project_api.Services.order_items;
using ec_project_api.Services.orders;
using ec_project_api.Services.inventory;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ec_project_api.Facades.orders
{
    public class OrderFacade
    {
        private readonly IOrderService _orderService;
        private readonly IOrderItemService _orderItemService;
        private readonly IProductVariantService _productVariantService;
        private readonly IShipService _shipService;
        private readonly IDiscountService _discountService;
        private readonly IMapper _mapper;
        private readonly DataContext _context; // để xử lý transaction
        private readonly IStatusService _statusService;
        private readonly IBatchInventoryService _batchInventoryService;

        public OrderFacade(
            IOrderService orderService,
            IOrderItemService orderItemService,
            IProductVariantService productVariantService,
            IMapper mapper,
            IStatusService statusService,
            IDiscountService discountService,
            IShipService shipService,
            DataContext context,
            IBatchInventoryService batchInventoryService)
        {
            _orderService = orderService;
            _orderItemService = orderItemService;
            _productVariantService = productVariantService;
            _mapper = mapper;
            _context = context;
            _statusService = statusService;
            _discountService = discountService;
            _shipService = shipService;
            _batchInventoryService = batchInventoryService;
        }
        public async Task<IEnumerable<OrderDetailDto>> GetAllAsync()
        {

            var orders = await _orderService.GetAllAsync();
            var result = _mapper.Map<IEnumerable<OrderDetailDto>>(orders);

            return result;
        }
        private static Expression<Func<Order, bool>> BuildOrderFilter(OrderFilter filter)
        {
            return o =>
                (string.IsNullOrEmpty(filter.StatusName) ||
                    (o.Status != null && o.Status.Name == filter.StatusName && o.Status.EntityType == EntityVariables.Order)) &&
                (string.IsNullOrEmpty(filter.Search) ||
                    o.OrderId.ToString().Contains(filter.Search) ||
                    (o.User != null && o.User.FullName.Contains(filter.Search)));
        }

        public async Task<PagedResult<OrderDetailDto>> GetAllPagedAsync(OrderFilter filter)
        {
            var options = new QueryOptions<Order>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Includes = { o => o.User, o => o.Status, o => o.Ship, o => o.Payment, o => o.OrderItems },
                Filter = BuildOrderFilter(filter)
            };

            // Include deep relations for items -> product variant -> product, size
            options.IncludeThen.Add(q => q
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductVariant)
                        .ThenInclude(pv => pv.Product));

            options.IncludeThen.Add(q => q
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductVariant)
                        .ThenInclude(pv => pv.Size));

            var pagedResult = await _orderService.GetAllPagedAsync(options);

            var dtoList = _mapper.Map<IEnumerable<OrderDetailDto>>(pagedResult.Items);
            
            return new PagedResult<OrderDetailDto>
            {
                Items = dtoList,
                TotalCount = pagedResult.TotalCount,
                TotalPages = pagedResult.TotalPages,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
        
        public async Task<OrderDetailDto> CreateOrderAsync(OrderCreateRequest request)
        {
            if (request.Items == null || !request.Items.Any())
                throw new InvalidOperationException(OrderMessages.OrderMustHaveAtLeastOneProduct);

            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // 1️⃣ Xử lý sản phẩm và tính tổng tiền
                    var (orderItems, totalAmount) = await ProcessOrderItemsAsync(request.Items);

                    // 2️⃣ Xử lý phí ship
                    var shippingFee = await CalculateShippingFeeAsync(request.ShipId, request.IsFreeShip);

                    // 3️⃣ Xử lý mã giảm giá
                    var discountAmount = await ApplyDiscountAsync(request.DiscountId, totalAmount);

                    // 4️⃣ Tính tổng cuối cùng
                    var finalAmount = totalAmount - discountAmount + shippingFee;

                    // 5️⃣ Lấy trạng thái Pending
                    var statusPending = await _statusService.FirstOrDefaultAsync(
                        s => s.EntityType == EntityVariables.Order && s.Name == StatusVariables.Pending)
                        ?? throw new InvalidOperationException(StatusMessages.StatusNotFound);

                    // 6️⃣ Tạo Order
                    var order = await CreateOrderEntityAsync(request, finalAmount, shippingFee, statusPending.StatusId);
                    await _orderService.CreateOrderAsync(order);
                    var createdOrder = await _orderService.GetByIdAsync(order.OrderId, new QueryOptions<Order>
                    {
                        Includes = { o => o.User, o => o.Status, o => o.Ship }
                    });

                    // 7️⃣ Tạo OrderItems
                    foreach (var item in orderItems)
                        item.OrderId = order.OrderId;

                    await _orderItemService.CreateOrderItemsAsync(orderItems);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    // 8️⃣ Trả về DTO kết quả
                    var ship = request.ShipId.HasValue ? await _shipService.GetByIdAsync(request.ShipId.Value) : null;
                    return BuildOrderDetailDto(createdOrder, orderItems, statusPending, ship);
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, short newStatusId)
        {
            var currentOrder = await _orderService.GetByIdAsync(orderId)
                ?? throw new KeyNotFoundException(OrderMessages.OrderNotFound);

            var currentStatus = await _statusService.GetByIdAsync(currentOrder.StatusId)
                ?? throw new InvalidOperationException(StatusMessages.StatusNotFound);
            // Nếu trạng thái hiện tại là "Delivered" thì không được phép thay đổi trạng thái nữa
            if (currentStatus.Name == StatusVariables.Delivered)
                throw new InvalidOperationException(OrderMessages.FinalStatusCannotChange);

            var nextStatus = await _statusService.GetByIdAsync(newStatusId)
                ?? throw new InvalidOperationException(StatusMessages.StatusNotFound);

            // Lấy danh sách trạng thái hợp lệ của EntityType = "Order"
            var options = new QueryOptions<Status>
            {
                Filter = s => s.EntityType == EntityVariables.Order
            };
            var validStatuses = await _statusService.GetAllAsync(options);

            if (!validStatuses.Any(s => s.StatusId == newStatusId))
                throw new InvalidOperationException(OrderMessages.InvalidStatusTransition);

            currentOrder.Status = nextStatus;

            return await _orderService.UpdateOrderStatusAsync(orderId, newStatusId);
        }
        public async Task<bool> AutoUpdateNextStatusAsync(int orderId)
        {
            var order = await _orderService.GetByIdAsync(orderId)
                ?? throw new KeyNotFoundException(OrderMessages.OrderNotFound);

            var currentStatus = await _statusService.GetByIdAsync(order.StatusId)
                ?? throw new InvalidOperationException(StatusMessages.StatusNotFound);

            // Bảng map trạng thái kế tiếp
            var nextStatusMap = new Dictionary<string, string>
            {
                { StatusVariables.Pending, StatusVariables.Confirmed },
                { StatusVariables.Confirmed, StatusVariables.Processing },
                { StatusVariables.Processing, StatusVariables.Shipped },
                { StatusVariables.Shipped, StatusVariables.Delivered }
            };

            if (!nextStatusMap.TryGetValue(currentStatus.Name, out var nextStatusName))
                throw new InvalidOperationException(string.Format(OrderMessages.StatusCannotTransitionFrom, currentStatus.DisplayName));

            var nextStatus = await _statusService.GetByNameAndEntityTypeAsync(nextStatusName, EntityVariables.Order)
                ?? throw new InvalidOperationException(StatusMessages.StatusNotFound);

            var updated = await _orderService.UpdateOrderStatusAsync(orderId, nextStatus.StatusId);

            return updated;
        }
        /// <summary>
        /// Hủy đơn hàng và hoàn trả sản phẩm về các lô theo LIFO
        /// </summary>
        public async Task<bool> CancelOrderAsync(int orderId)
        {
            var order = await _orderService.GetByIdAsync(orderId)
                ?? throw new KeyNotFoundException(OrderMessages.OrderNotFound);

            // Lấy trạng thái hiện tại
            var currentStatus = await _statusService.GetByIdAsync(order.StatusId)
                ?? throw new InvalidOperationException(StatusMessages.StatusNotFound);

            // Nếu đơn hàng đã xác nhận
            if (currentStatus.Name != StatusVariables.Pending)
                throw new InvalidOperationException(OrderMessages.OrderCannotBeCancelAfterPending);


            // ✅ Hoàn trả hàng về các lô trước khi hủy đơn
            var orderItems = await _orderItemService.GetOrderItemsByOrderIdAsync(orderId);
            
            foreach (var item in orderItems)
            {
                // Hoàn trả số lượng về các lô theo LIFO (ngược với lúc trừ)
                await _batchInventoryService.ReturnToBatchesAsync(item.ProductVariantId, item.Quantity);

                // Cập nhật lại StockQuantity của ProductVariant
                var variant = await _productVariantService.GetByIdAsync(item.ProductVariantId);
                if (variant != null)
                {
                    variant.StockQuantity = await _batchInventoryService.GetAvailableStockAsync(item.ProductVariantId);
                    variant.UpdatedAt = DateTime.UtcNow;
                    await _productVariantService.UpdateAsync(variant);
                }
            }

            // Hoàn trả UsedCount cho Discount (nếu có)
            if (order.DiscountId.HasValue)
            {
                var discount = await _context.Discounts.FindAsync(order.DiscountId.Value);
                if (discount != null && discount.UsedCount > 0)
                {
                    discount.UsedCount -= 1;
                    discount.UpdatedAt = DateTime.UtcNow;
                    _context.Discounts.Update(discount);
                    await _context.SaveChangesAsync();
                }
            }

            // Lấy trạng thái Cancelled
            var cancelledStatus = await _statusService.FirstOrDefaultAsync(
                s => s.EntityType == EntityVariables.Order && s.Name == StatusVariables.Cancelled
            ) ?? throw new InvalidOperationException(OrderMessages.CancelledStatusNotFound);

            // Cập nhật trạng thái
            return await _orderService.UpdateOrderStatusAsync(orderId, cancelledStatus.StatusId);
        }



        /// <summary>
        /// Xóa đơn hàng Draft và hoàn trả sản phẩm về các lô
        /// </summary>
        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            var order = await _orderService.GetByIdAsync(orderId)
                ?? throw new KeyNotFoundException(OrderMessages.OrderNotFound);

            if (order.Status.Name != StatusVariables.Draft)
                throw new InvalidOperationException(OrderMessages.OrderCannotBeDeleted);

            // 1️⃣ Lấy danh sách OrderItems để hoàn trả tồn kho
            var orderItems = await _orderItemService.GetOrderItemsByOrderIdAsync(orderId);

            // 2️⃣ Hoàn trả tồn kho về các lô
            foreach (var item in orderItems)
            {
                // Hoàn trả số lượng về các lô
                await _batchInventoryService.ReturnToBatchesAsync(item.ProductVariantId, item.Quantity);

                // Cập nhật lại StockQuantity của ProductVariant
                var variant = await _productVariantService.GetByIdAsync(item.ProductVariantId);
                if (variant != null)
                {
                    variant.StockQuantity = await _batchInventoryService.GetAvailableStockAsync(item.ProductVariantId);
                    variant.UpdatedAt = DateTime.UtcNow;
                    await _productVariantService.UpdateAsync(variant);
                }
            }

            // 3️⃣ Hoàn trả UsedCount cho Discount (nếu có)
            if (order.DiscountId.HasValue)
            {
                var discount = await _context.Discounts.FindAsync(order.DiscountId.Value);
                if (discount != null && discount.UsedCount > 0)
                {
                    discount.UsedCount -= 1;
                    discount.UpdatedAt = DateTime.UtcNow;
                    _context.Discounts.Update(discount);
                }
            }

            // 4️⃣ Lưu thay đổi tồn kho và discount
            await _productVariantService.SaveChangesAsync();
            await _context.SaveChangesAsync();

            // 5️⃣ Xóa Order (cascade sẽ tự động xóa OrderItems nếu có cấu hình)
            return await _orderService.DeleteAsync(order);
        }
        public async Task<OrderDetailDto> GetOrderByIdAsync(int orderId)
        {
            var order = await _orderService.GetByIdAsync(orderId)
                ?? throw new KeyNotFoundException(OrderMessages.OrderNotFound);
            var orderDto = _mapper.Map<OrderDetailDto>(order);
            return orderDto;
        }

        public async Task<IEnumerable<OrderDetailDto>> GetOrdersByUserIdAsync(int userId)
        {
            var options = new QueryOptions<Order>
            {
                Filter = o => o.UserId == userId
            };
            var orders = await _orderService.GetAllAsync(options);
            var result = _mapper.Map<IEnumerable<OrderDetailDto>>(orders);
            return result;
        }
        // Helper
        /// <summary>
        /// Xử lý OrderItems với logic FIFO batch inventory
        /// Trừ hàng từ các lô theo thứ tự nhập trước - xuất trước
        /// </summary>
        private async Task<(List<OrderItem> items, decimal totalAmount)> ProcessOrderItemsAsync(IEnumerable<OrderItemCreateRequest> items)
        {
            decimal totalAmount = 0m;
            var orderItems = new List<OrderItem>();

            foreach (var item in items)
            {
                var variant = await _productVariantService.GetByIdAsync(item.ProductVariantId)
                    ?? throw new InvalidOperationException(string.Format(OrderMessages.ProductVariantNotFoundById, item.ProductVariantId));

                // ✅ Kiểm tra tồn kho từ các lô đang active (is_pushed = true)
                var availableStock = await _batchInventoryService.GetAvailableStockAsync(item.ProductVariantId);
                
                if (availableStock < item.Quantity)
                {
                    // Kiểm tra xem có thể kích hoạt lô tiếp theo không
                    var totalInactiveBatches = await _context.PurchaseOrderItems
                        .Where(poi => poi.ProductVariantId == item.ProductVariantId && !poi.IsPushed)
                        .SumAsync(poi => (int)poi.Quantity);

                    if (availableStock + totalInactiveBatches < item.Quantity)
                    {
                        throw new InvalidOperationException(
                            string.Format(OrderMessages.OrderItemOutOfStockWithSku, variant.Sku) + 
                            $" (Khả dụng: {availableStock}, Tồn kho chưa kích hoạt: {totalInactiveBatches})");
                    }
                }

                // ✅ Trừ hàng từ các lô theo FIFO
                var batchDeductions = await _batchInventoryService.DeductFromBatchesAsync(
                    item.ProductVariantId, 
                    item.Quantity);

                // ✅ Tính giá bán trung bình từ các lô (theo tỷ lệ số lượng từng lô)
                decimal totalPrice = 0m;
                foreach (var batch in batchDeductions)
                {
                    totalPrice += batch.SellingPrice * batch.QuantityDeducted;
                }
                var averagePrice = totalPrice / item.Quantity;
                var subTotal = averagePrice * item.Quantity;
                totalAmount += subTotal;

                // ✅ Cập nhật tồn kho của ProductVariant (chỉ để hiển thị)
                // Tồn kho thực tế được quản lý ở PurchaseOrderItem
                variant.StockQuantity = await _batchInventoryService.GetAvailableStockAsync(item.ProductVariantId);
                variant.UpdatedAt = DateTime.UtcNow;
                await _productVariantService.UpdateAsync(variant);

                orderItems.Add(new OrderItem
                {
                    ProductVariantId = variant.ProductVariantId,
                    Quantity = item.Quantity,
                    Price = averagePrice, // Giá bán trung bình từ các lô
                    SubTotal = subTotal
                });
            }

            return (orderItems, totalAmount);
        }
        private async Task<decimal> CalculateShippingFeeAsync(byte? shipId, bool isFreeShip)
        {
            if (shipId.HasValue && !isFreeShip)
            {
                var ship = await _shipService.GetByIdAsync(shipId.Value)
                    ?? throw new InvalidOperationException(OrderMessages.ShippingMethodNotFound);
                return ship.BaseCost;
            }
            return 0m;
        }
        private async Task<decimal> ApplyDiscountAsync(byte? discountId, decimal totalAmount)
        {
            if (!discountId.HasValue) return 0m;

            var discount = await _discountService.GetByIdAsync(discountId.Value)
                ?? throw new InvalidOperationException(OrderMessages.DiscountInvalid);

            var now = DateTime.UtcNow;
            if (discount.StartAt.HasValue && now < discount.StartAt.Value)
                throw new InvalidOperationException(OrderMessages.DiscountNotStarted);

            if (discount.EndAt.HasValue && now > discount.EndAt.Value)
                throw new InvalidOperationException(OrderMessages.DiscountExpired);

            if (discount.UsageLimit.HasValue && discount.UsedCount >= discount.UsageLimit.Value)
                throw new InvalidOperationException(OrderMessages.DiscountUsageExceeded);

            if (totalAmount < discount.MinOrderAmount)
                throw new InvalidOperationException(string.Format(OrderMessages.DiscountMinOrderAmount, discount.MinOrderAmount));

            decimal discountAmount = discount.DiscountType.ToLower() switch
            {
                "percentage" => totalAmount * (discount.DiscountValue / 100),
                "fixed" => discount.DiscountValue,
                _ => 0m
            };

            if (discount.MaxDiscountAmount.HasValue)
                discountAmount = Math.Min(discountAmount, discount.MaxDiscountAmount.Value);

            discount.UsedCount += 1;
            discount.UpdatedAt = DateTime.UtcNow;
            _context.Discounts.Update(discount);

            return discountAmount;
        }
        private Task<Order> CreateOrderEntityAsync(OrderCreateRequest request, decimal total, decimal shipFee, short statusId)
        {
            return Task.FromResult(new Order
            {
                UserId = request.UserId,
                DiscountId = request.DiscountId,
                AddressInfo = request.AddressInfo,
                ShipId = request.ShipId,
                TotalAmount = total,
                ShippingFee = shipFee,
                IsFreeShip = request.IsFreeShip,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                StatusId = statusId
            });
        }
        private OrderDetailDto BuildOrderDetailDto(Order order, IEnumerable<OrderItem> orderItems, Status status, Ship? ship)
        {
            return new OrderDetailDto
            {
                OrderId = order.OrderId,
                User = order.User != null
                ? new UserOrderDto
                {
                    UserId = order.User.UserId,
                    FullName = order.User.FullName
                }
                : null!,
                AddressInfo = order.AddressInfo,
                TotalAmount = order.TotalAmount,
                ShippingFee = order.ShippingFee,
                IsFreeShip = order.IsFreeShip,
                CreatedAt = order.CreatedAt,
                Ship = ship != null ? new ShipOrderDto
                {
                    ShipId = ship.ShipId,
                    CorpName = ship.CorpName
                } : null!,
                Status = new StatusOrderDto
                {
                    StatusId = status.StatusId,
                    Name = status.Name
                },
                Items = orderItems.Select(oi => new OrderItemsDto
                {
                    ProductName = oi.ProductVariant!.Product!.Name,
                    Sku = oi.ProductVariant!.Sku,
                    Size = oi.ProductVariant!.Size!.Name,
                    Quantity = oi.Quantity,
                    Price = oi.Price,
                    SubTotal = oi.SubTotal
                })
            };
        }

    }
}
