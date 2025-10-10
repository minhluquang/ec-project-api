using ec_project_api.Interfaces.Orders;
using ec_project_api.Models;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services.order_items {
    public class OrderItemService : BaseService<OrderItem, int>, IOrderItemService {
        private readonly IOrderItemRepository _orderItemRepository;

        public OrderItemService(IOrderItemRepository orderItemRepository) : base(orderItemRepository) {
            _orderItemRepository = orderItemRepository;
        }

        // Add custom methods implementation here if needed
    }
}