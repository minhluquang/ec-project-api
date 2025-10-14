using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.Interfaces.Orders;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;
using Microsoft.EntityFrameworkCore;

namespace ec_project_api.Services.orders
{
    public class OrderService : BaseService<Order, int>, IOrderService
    {
        private readonly IOrderRepository _orderRepository;
       

        public OrderService(IOrderRepository orderRepository)
            : base(orderRepository)
        {
            _orderRepository = orderRepository;
          
        }

        // ✅ Lấy tất cả đơn hàng (kèm User, Status, OrderItems)
        public override async Task<IEnumerable<Order>> GetAllAsync(QueryOptions<Order>? options = null)
        {
            options ??= new QueryOptions<Order>();

            // Include các quan hệ chính
            options.Includes.Add(o => o.User);
            options.Includes.Add(o => o.Status);
            options.Includes.Add(o => o.Discount);
            options.Includes.Add(o => o.Ship);
            options.Includes.Add(o => o.Payment);
            options.Includes.Add(o => o.OrderItems);

            // ✅ Include sâu các mối quan hệ của OrderItems
            options.IncludeThen.Add(q => q
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductVariant)
                        .ThenInclude(pv => pv.Product));

            // ✅ Thêm Include cho Size (cấp sâu hơn)
            options.IncludeThen.Add(q => q
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductVariant)
                        .ThenInclude(pv => pv.Size));
            var orders = await _orderRepository.GetAllAsync(options);
            return orders;
        }

        // ✅ Lấy đơn hàng theo ID (kèm các thông tin liên quan)
        public override async Task<Order?> GetByIdAsync(int id, QueryOptions<Order>? options = null)
        {
            options ??= new QueryOptions<Order>();

            options.Includes.Add(o => o.User);
            options.Includes.Add(o => o.Status);
            options.Includes.Add(o => o.Discount);
            options.Includes.Add(o => o.Ship);
            options.Includes.Add(o => o.Payment);
            options.Includes.Add(o => o.OrderItems);

            // ✅ Dùng IncludeThen cho các cấp sâu
            options.IncludeThen.Add(q => q
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductVariant)
                        .ThenInclude(pv => pv.Product));

            return await _orderRepository.GetByIdAsync(id, options);
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync();
            return order;
        }
        public async Task<bool> UpdateOrderStatusAsync(int orderId, int newStatusId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new InvalidOperationException(OrderMessages.OrderNotFound);

            order.StatusId = newStatusId;
            order.UpdatedAt = DateTime.UtcNow;

            if (order.Status?.Name == "Shipped")
                order.ShippedAt = DateTime.UtcNow;

            if (order.Status?.Name == "Delivered")
                order.DeliveryAt = DateTime.UtcNow;

            return await UpdateAsync(order);
        }
    }
}
