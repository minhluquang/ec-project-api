using ec_project_api.Models;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services.orders {
    public interface IOrderService : IBaseService<Order, int> {
        Task<Order> CreateOrderAsync(Order order);
        Task<bool> UpdateOrderStatusAsync(int orderId, int newStatusId);
    }
}                                                                       