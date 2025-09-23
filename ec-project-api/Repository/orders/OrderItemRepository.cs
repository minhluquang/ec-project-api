using ec_project_api.Interfaces.Orders;
using ec_project_api.Models;

public class OrderItemRepository : Repository<OrderItem, int>, IOrderItemRepository
{
    public OrderItemRepository(DataContext context) : base(context) { }
}
