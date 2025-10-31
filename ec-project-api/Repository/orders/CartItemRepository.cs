using ec_project_api.Interfaces.Orders;
using ec_project_api.Models;

public class CartRepository : Repository<Cart, int>, ICartRepository
{
    public CartRepository(DataContext context) : base(context) { }
}
