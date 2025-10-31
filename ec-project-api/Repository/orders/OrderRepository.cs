using ec_project_api.Interfaces.Orders;
using ec_project_api.Models;

public class OrderRepository : Repository<Order, int>, IOrderRepository
{
    public OrderRepository(DataContext context) : base(context) { }
}
