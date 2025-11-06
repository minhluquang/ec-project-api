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
        private readonly DataContext _context;
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
                
                    var (orderItems, totalAmount) = await ProcessOrderItemsAsync(request.Items);

                
                    var shippingFee = await CalculateShippingFeeAsync(request.ShipId, request.IsFreeShip);

                
                    var discountAmount = await ApplyDiscountAsync(request.DiscountId, totalAmount);

                
                    var finalAmount = totalAmount - discountAmount + shippingFee;

                
                    var statusPending = await _statusService.FirstOrDefaultAsync(
                        s => s.EntityType == EntityVariables.Order && s.Name == StatusVariables.Pending)
                        ?? throw new InvalidOperationException(StatusMessages.StatusNotFound);

                
                    var order = await CreateOrderEntityAsync(request, finalAmount, shippingFee, statusPending.StatusId);
                    await _orderService.CreateOrderAsync(order);
                    var createdOrder = await _orderService.GetByIdAsync(order.OrderId, new QueryOptions<Order>
                    {
                        Includes = { o => o.User, o => o.Status, o => o.Ship }
                    });

                
                    foreach (var item in orderItems)
                        item.OrderId = order.OrderId;

                    await _orderItemService.CreateOrderItemsAsync(orderItems);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                
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
        
            if (currentStatus.Name == StatusVariables.Delivered)
                throw new InvalidOperationException(OrderMessages.FinalStatusCannotChange);

            var nextStatus = await _statusService.GetByIdAsync(newStatusId)
                ?? throw new InvalidOperationException(StatusMessages.StatusNotFound);

        
            var options = new QueryOptions<Status>
            {
                Filter = s => s.EntityType == EntityVariables.Order
            };
            var validStatuses = await _statusService.GetAllAsync(options);

            if (!validStatuses.Any(s => s.StatusId == newStatusId))
                throw new InvalidOperationException(OrderMessages.InvalidStatusTransition);

        
            if (nextStatus.Name == StatusVariables.Processing && currentStatus.Name != StatusVariables.Processing)
            {
                await DeductInventoryForOrderAsync(orderId);
            }

            currentOrder.Status = nextStatus;

            return await _orderService.UpdateOrderStatusAsync(orderId, newStatusId);
        }
        public async Task<bool> AutoUpdateNextStatusAsync(int orderId)
        {
            var order = await _orderService.GetByIdAsync(orderId)
                ?? throw new KeyNotFoundException(OrderMessages.OrderNotFound);

            var currentStatus = await _statusService.GetByIdAsync(order.StatusId)
                ?? throw new InvalidOperationException(StatusMessages.StatusNotFound);

        
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
            if (nextStatus.Equals(StatusVariables.Shipped))
                order.ShippedAt = DateTime.UtcNow;

            if (nextStatus.Equals(StatusVariables.Delivered))
                order.DeliveryAt = DateTime.UtcNow;
            if (nextStatus.Name == StatusVariables.Processing && currentStatus.Name != StatusVariables.Processing)
            {
                await DeductInventoryForOrderAsync(orderId);
            }


            var updated = await _orderService.UpdateOrderStatusAsync(orderId, nextStatus.StatusId);

            return updated;
        }
    
    
    
        public async Task<bool> CancelOrderAsync(int orderId)
        {
            var order = await _orderService.GetByIdAsync(orderId)
                ?? throw new KeyNotFoundException(OrderMessages.OrderNotFound);

        
            var currentStatus = await _statusService.GetByIdAsync(order.StatusId)
                ?? throw new InvalidOperationException(StatusMessages.StatusNotFound);

        
            if (currentStatus.Name != StatusVariables.Pending)
                throw new InvalidOperationException(OrderMessages.OrderCannotBeCancelAfterPending);

        
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

        
            var cancelledStatus = await _statusService.FirstOrDefaultAsync(
                s => s.EntityType == EntityVariables.Order && s.Name == StatusVariables.Cancelled
            ) ?? throw new InvalidOperationException(OrderMessages.CancelledStatusNotFound);

        
            return await _orderService.UpdateOrderStatusAsync(orderId, cancelledStatus.StatusId);
        }



    
    
    
        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            var order = await _orderService.GetByIdAsync(orderId)
                ?? throw new KeyNotFoundException(OrderMessages.OrderNotFound);

            if (order.Status.Name != StatusVariables.Draft)
                throw new InvalidOperationException(OrderMessages.OrderCannotBeDeleted);

        

        
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

        
            await _context.SaveChangesAsync();

        
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
                Filter = o => o.UserId == userId,
                OrderBy = q => q.OrderByDescending(o => o.OrderId)
            };
            var orders = await _orderService.GetAllAsync(options);
            var result = _mapper.Map<IEnumerable<OrderDetailDto>>(orders);
            return result;
        }
    
    
    
    
    
    
        private async Task<(List<OrderItem> items, decimal totalAmount)> ProcessOrderItemsAsync(IEnumerable<OrderItemCreateRequest> items)
        {
            decimal totalAmount = 0m;
            var orderItems = new List<OrderItem>();

            foreach (var item in items)
            {
                var variant = await _productVariantService.GetByIdAsync(item.ProductVariantId)
                    ?? throw new InvalidOperationException(string.Format(OrderMessages.ProductVariantNotFoundById, item.ProductVariantId));

            
                var availableStock = await _batchInventoryService.GetAvailableStockAsync(item.ProductVariantId);
                
                if (availableStock < item.Quantity)
                {
                
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

            
                var product = await _context.Products.FindAsync(variant.ProductId);
                if (product == null || product.BasePrice <= 0)
                {
                    throw new InvalidOperationException(
                        $"Sản phẩm {variant.Sku} chưa có giá bán. Vui lòng cập nhật giá.");
                }

            
                var sellingPrice = product.BasePrice;
                if (product.DiscountPercentage.HasValue && product.DiscountPercentage.Value > 0)
                {
                    sellingPrice = product.BasePrice * (1 - product.DiscountPercentage.Value / 100);
                }

                var subTotal = sellingPrice * item.Quantity;
                totalAmount += subTotal;

                orderItems.Add(new OrderItem
                {
                    ProductVariantId = variant.ProductVariantId,
                    Quantity = item.Quantity,
                    Price = sellingPrice,
                    SubTotal = subTotal
                });
            }

            return (orderItems, totalAmount);
        }

        private async Task DeductInventoryForOrderAsync(int orderId)
        {
            var orderItems = await _orderItemService.GetOrderItemsByOrderIdAsync(orderId);

            foreach (var item in orderItems)
            {

                await _batchInventoryService.DeductFromBatchesAsync(
                    item.ProductVariantId,
                    item.Quantity);
            }
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
            // Lấy thông tin discount
            var discount = await _discountService.GetByIdAsync(discountId.Value)
                ?? throw new InvalidOperationException(OrderMessages.DiscountInvalid);

            var now = DateTime.UtcNow;
            var inactiveStatus = await _statusService.FirstOrDefaultAsync(
                s => s.EntityType == EntityVariables.Discount && s.Name == StatusVariables.Inactive
            ) ?? throw new InvalidOperationException(string.Format(StatusMessages.StatusNotFound));

           await _discountService.CheckAndUpdateDiscountStatusByIdAsync((int) discountId, inactiveStatus.StatusId);

            if (discount.StartAt.HasValue && now < discount.StartAt.Value)
                throw new InvalidOperationException(OrderMessages.DiscountNotStarted);

            if (discount.EndAt.HasValue && now > discount.EndAt.Value)
                throw new InvalidOperationException(OrderMessages.DiscountExpired);

            if (discount.UsageLimit.HasValue && discount.UsedCount >= discount.UsageLimit.Value)
                throw new InvalidOperationException(OrderMessages.DiscountUsageExceeded);

            if (totalAmount < discount.MinOrderAmount)
                throw new InvalidOperationException(string.Format(
                    OrderMessages.DiscountMinOrderAmount, discount.MinOrderAmount
                ));

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

            
            if ((discount.UsageLimit.HasValue && discount.UsedCount >= discount.UsageLimit.Value) ||
                (discount.EndAt.HasValue && discount.EndAt.Value.Date < now.Date))
            {
                discount.StatusId = inactiveStatus.StatusId;
            }

            _context.Discounts.Update(discount);
            await _context.SaveChangesAsync();

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
