using ec_project_api.Constants.variables;
using ec_project_api.Interfaces.Orders;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;
using ec_project_api.Services.order_items;

namespace ec_project_api.Services.orders
{
    public class OrderItemService : BaseService<OrderItem, int>, IOrderItemService
    {
        private readonly IOrderItemRepository _orderItemRepository;

        public OrderItemService(IOrderItemRepository orderItemRepository)
            : base(orderItemRepository)
        {
            _orderItemRepository = orderItemRepository;
        }

        // ✅ Lấy tất cả OrderItem kèm ProductVariant & Product
        public override async Task<IEnumerable<OrderItem>> GetAllAsync(QueryOptions<OrderItem>? options = null)
        {
            options ??= new QueryOptions<OrderItem>();
            options.Includes.Add(oi => oi.ProductVariant);
            options.Includes.Add(oi => oi.ProductVariant!.Product);
            options.Includes.Add(oi => oi.ProductVariant!.Size);
            return await _orderItemRepository.GetAllAsync(options);
        }

        // ✅ Lấy OrderItem theo ID
        public override async Task<OrderItem?> GetByIdAsync(int id, QueryOptions<OrderItem>? options = null)
        {
            options ??= new QueryOptions<OrderItem>();
            options.Includes.Add(oi => oi.ProductVariant);
            options.Includes.Add(oi => oi.ProductVariant!.Product);
            options.Includes.Add(oi => oi.ProductVariant!.Size);
            return await _orderItemRepository.GetByIdAsync(id, options);
        }
        public async Task<bool> CreateOrderItemsAsync(IEnumerable<OrderItem> orderItems)
        {
            foreach (var item in orderItems)
            {
                await _orderItemRepository.AddAsync(item);
            }
            return await _orderItemRepository.SaveChangesAsync() > 0;
        }

        // ✅ Cập nhật số lượng hoặc giá của OrderItem
        public async Task<bool> UpdateItemAsync(int id, short newQuantity, decimal newPrice)
        {
            var orderItem = await _orderItemRepository.GetByIdAsync(id);
            if (orderItem == null)
                return false;

            orderItem.Quantity = newQuantity;
            orderItem.Price = newPrice;
            orderItem.SubTotal = newQuantity * newPrice;
            orderItem.UpdatedAt = DateTime.UtcNow;

            await _orderItemRepository.UpdateAsync(orderItem);
            return await _orderItemRepository.SaveChangesAsync() > 0;
        }

        // ✅ Lấy tất cả OrderItem theo OrderId
        public async Task<IEnumerable<OrderItem>> GetOrderItemsByOrderIdAsync(int orderId)
        {
            var options = new QueryOptions<OrderItem>
            {
                Filter = oi => oi.OrderId == orderId
            };
            options.Includes.Add(oi => oi.ProductVariant);
            options.Includes.Add(oi => oi.ProductVariant!.Product);
            options.Includes.Add(oi => oi.ProductVariant!.Size);

            return await _orderItemRepository.GetAllAsync(options);
        }
        
        public async Task<int> GetSoldQuantityByProductIdAsync(int productId)
        {
            var options = new QueryOptions<OrderItem>
            {
                Filter = oi =>
                    oi.ProductVariant != null &&
                    oi.ProductVariant.ProductId == productId &&
                    oi.Order != null &&
                    oi.Order.Status != null &&
                    oi.Order.Status.Name == StatusVariables.Delivered
            };

            options.Includes.Add(oi => oi.Order);

            var orderItems = await _orderItemRepository.GetAllAsync(options);
            return orderItems.Sum(oi => oi.Quantity);
        }
    }
}
