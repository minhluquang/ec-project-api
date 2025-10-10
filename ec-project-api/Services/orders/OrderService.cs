using ec_project_api.Interfaces.Orders;
using ec_project_api.Models;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services.orders {
    public class OrderService : BaseService<Order, int>, IOrderService {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository) : base(orderRepository) {
            _orderRepository = orderRepository;
        }

        // Add custom methods implementation here if needed
    }
}