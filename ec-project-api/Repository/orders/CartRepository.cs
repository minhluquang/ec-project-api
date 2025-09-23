using ec_project_api.Interfaces.Orders;
using ec_project_api.Models;

public class CartItemRepository : Repository<CartItem, int>, ICartItemRepository
{
    public CartItemRepository(DataContext context) : base(context) { }
}
