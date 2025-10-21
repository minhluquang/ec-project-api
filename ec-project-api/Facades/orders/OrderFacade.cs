using AutoMapper;
using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.Dtos.request.orders;
using ec_project_api.Dtos.response.orders;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services;
using ec_project_api.Services.order_items;
using ec_project_api.Services.orders;
using Microsoft.EntityFrameworkCore;

namespace ec_project_api.Facades.orders
{
    public class OrderFacade
    {
        private readonly IOrderService _orderService;
        private readonly IOrderItemService _orderItemService;
        private readonly IProductVariantService _productVariantService;
        private readonly IMapper _mapper;
        private readonly DataContext _context; // để xử lý transaction
        private readonly IStatusService _statusService;
        public OrderFacade(
            IOrderService orderService,
            IOrderItemService orderItemService,
            IProductVariantService productVariantService,
            IMapper mapper,
            IStatusService statusService,
            DataContext context)
        {
            _orderService = orderService;
            _orderItemService = orderItemService;
            _productVariantService = productVariantService;
            _mapper = mapper;
            _context = context;
            _statusService = statusService;
        }
        public async Task<IEnumerable<OrderDetailDto>> GetAllAsync()
        {

            var orders = await _orderService.GetAllAsync();
            var result = _mapper.Map<IEnumerable<OrderDetailDto>>(orders);

            return result;
        }
        public async Task<OrderDetailDto> CreateOrderAsync(OrderCreateRequest request)
        {
            if (request.Items == null || !request.Items.Any())
                throw new InvalidOperationException("Đơn hàng phải có ít nhất 1 sản phẩm.");

            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    decimal totalAmount = 0m;
                    var orderItems = new List<OrderItem>();

                    // 1️⃣ Duyệt qua từng sản phẩm trong đơn hàng
                    foreach (var item in request.Items)
                    {
                        var variant = await _productVariantService.GetByIdAsync(item.ProductVariantId)
                            ?? throw new InvalidOperationException($"Không tìm thấy biến thể sản phẩm ID: {item.ProductVariantId}");

                        if (variant.StockQuantity < item.Quantity)
                            throw new InvalidOperationException($"Sản phẩm {variant.Sku} không đủ hàng trong kho.");

                        var price = variant.Product!.BasePrice;
                        var subTotal = price * item.Quantity;
                        totalAmount += subTotal;

                        // Trừ tồn kho
                        variant.StockQuantity -= item.Quantity;
                        variant.UpdatedAt = DateTime.UtcNow;
                        await _productVariantService.UpdateAsync(variant);

                        orderItems.Add(new OrderItem
                        {
                            ProductVariantId = variant.ProductVariantId,
                            Quantity = item.Quantity,
                            Price = price,
                            SubTotal = subTotal
                        });
                    }

                    // 2️⃣ Tính phí ship
                    decimal shippingFee = 0m;
                    if (request.ShipId.HasValue && !request.IsFreeShip)
                    {
                        var ship = await _context.Ships.FindAsync(request.ShipId.Value)
                            ?? throw new InvalidOperationException("Không tìm thấy phương thức giao hàng.");

                        shippingFee = ship.BaseCost;
                    }

                    // 3️⃣ Xử lý Discount (nếu có)
                    decimal discountAmount = 0m;
                    if (request.DiscountId.HasValue)
                    {
                        var discount = await _context.Discounts.FindAsync(request.DiscountId.Value)
                            ?? throw new InvalidOperationException("Mã giảm giá không hợp lệ.");

                        // Kiểm tra thời hạn
                        var now = DateTime.UtcNow;
                        if (discount.StartAt.HasValue && now < discount.StartAt.Value)
                            throw new InvalidOperationException("Mã giảm giá chưa có hiệu lực.");

                        if (discount.EndAt.HasValue && now > discount.EndAt.Value)
                            throw new InvalidOperationException("Mã giảm giá đã hết hạn.");

                        // Kiểm tra số lần sử dụng
                        if (discount.UsageLimit.HasValue && discount.UsedCount >= discount.UsageLimit.Value)
                            throw new InvalidOperationException("Mã giảm giá đã được sử dụng tối đa.");

                        // Kiểm tra giá trị đơn hàng tối thiểu
                        if (totalAmount < discount.MinOrderAmount)
                            throw new InvalidOperationException($"Đơn hàng chưa đạt mức tối thiểu {discount.MinOrderAmount:N0} để áp dụng mã giảm giá.");

                        // Áp dụng giảm giá
                        if (discount.DiscountType.ToLower() == "percentage")
                        {
                            discountAmount = totalAmount * (discount.DiscountValue / 100);
                            if (discount.MaxDiscountAmount.HasValue)
                                discountAmount = Math.Min(discountAmount, discount.MaxDiscountAmount.Value);
                        }
                        else if (discount.DiscountType.ToLower() == "fixed")
                        {
                            discountAmount = discount.DiscountValue;
                        }

                        // Tăng UsedCount
                        discount.UsedCount += 1;
                        discount.UpdatedAt = DateTime.UtcNow;
                        _context.Discounts.Update(discount);
                    }

                    // 4️⃣ Tính tổng tiền cuối cùng
                    var finalAmount = totalAmount - discountAmount + shippingFee;

                    var statusDraft = await _statusService.FirstOrDefaultAsync(s => s.EntityType == EntityVariables.Order && s.Name == StatusVariables.Draft) ??
                throw new InvalidOperationException(StatusMessages.StatusNotFound);

                    // 5️⃣ Tạo Order
                    var order = new Order
                    {
                        UserId = request.UserId,
                        DiscountId = request.DiscountId,
                        AddressInfo = request.AddressInfo,
                        ShipId = request.ShipId,
                        TotalAmount = finalAmount,
                        ShippingFee = shippingFee,
                        IsFreeShip = request.IsFreeShip,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        StatusId = statusDraft.StatusId
                    };

                    await _orderService.CreateOrderAsync(order);

                    // 6️⃣ Tạo OrderItems
                    foreach (var item in orderItems)
                    {
                        item.OrderId = order.OrderId;
                    }

                    await _orderItemService.CreateOrderItemsAsync(orderItems);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    // 7️⃣ Trả về DTO kết quả
                    var orderDto = new OrderDetailDto
                    {
                        OrderId = order.OrderId,
                        UserId = order.UserId,
                        AddressInfo = order.AddressInfo,
                        TotalAmount = order.TotalAmount,
                        ShippingFee = order.ShippingFee,
                        IsFreeShip = order.IsFreeShip,
                        CreatedAt = order.CreatedAt,
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

                    return orderDto;
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
            // Nếu trạng thái hiện tại là "Shipped" thì không được phép thay đổi trạng thái nữa
            if (currentStatus.Name == StatusVariables.Shipped)
                throw new InvalidOperationException(OrderMessages.FinalStatusCannotChange);

            var nextStatus = await _statusService.GetByIdAsync(newStatusId)
                ?? throw new InvalidOperationException(StatusMessages.StatusNotFound);

            // ✅ Lấy danh sách trạng thái hợp lệ của EntityType = "Order"
            var options = new QueryOptions<Status>
            {
                Filter = s => s.EntityType == EntityVariables.Order
            };
            var validStatuses = await _statusService.GetAllAsync(options);

            if (!validStatuses.Any(s => s.StatusId == newStatusId))
                throw new InvalidOperationException(OrderMessages.InvalidStatusTransition);

            // Cập nhật trạng thái
            currentOrder.Status = nextStatus;

            return await _orderService.UpdateOrderStatusAsync(orderId, newStatusId);
        }
        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            var order = await _orderService.GetByIdAsync(orderId)
                ?? throw new KeyNotFoundException(OrderMessages.OrderNotFound);

            if (order.Status.Name != StatusVariables.Draft)
                throw new InvalidOperationException(OrderMessages.OrderCannotBeDeleted);

            // 1️⃣ Lấy danh sách OrderItems để hoàn trả tồn kho
            var orderItems = await _orderItemService.GetOrderItemsByOrderIdAsync(orderId);

            // 2️⃣ Hoàn trả tồn kho cho các sản phẩm
            foreach (var item in orderItems)
            {
                var variant = await _productVariantService.GetByIdAsync(item.ProductVariantId);
                if (variant != null)
                {
                    variant.StockQuantity += item.Quantity;
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

    }
}
