using ec_project_api.Interfaces.Orders;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;
using Microsoft.EntityFrameworkCore;

namespace ec_project_api.Services.cart
{
    public class CartService : BaseService<Cart, int>, ICartService
    {
        private readonly ICartRepository _cartRepository;
        public CartService(ICartRepository cartRepository) : base(cartRepository)
        {
            _cartRepository = cartRepository;
        }
        public async Task<Cart?> GetCartWithItemsAsync(int userId,QueryOptions<Cart>? options = null)
        {
            options ??= new QueryOptions<Cart>();
            options.Includes.Add(c => c.CartItems);

            options.IncludeThen.Add(q => q
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.ProductVariant)
                        .ThenInclude(pv => pv.Product)
                            .ThenInclude(p => p.ProductImages));
            return await _cartRepository.FirstOrDefaultAsync(c => c.UserId == userId, options);
        }
    }
}
