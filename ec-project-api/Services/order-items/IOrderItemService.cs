using ec_project_api.Models;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services.order_items {
    public interface IOrderItemService : IBaseService<OrderItem, int> {
        Task<bool> CreateOrderItemsAsync(IEnumerable<OrderItem> orderItems);
        Task<IEnumerable<OrderItem>> GetOrderItemsByOrderIdAsync(int orderId);
        Task<int> GetSoldQuantityByProductIdAsync(int productId);
    }
}