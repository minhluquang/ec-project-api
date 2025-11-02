using ec_project_api.Dtos.response.pagination;
using ec_project_api.Interfaces.Orders;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;
using System.Linq.Expressions;

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
           options = options ?? new QueryOptions<Cart>();
           options.Includes.Add(c => c.CartItems);
            options.Filter = c => c.UserId == userId;
           return await _cartRepository.GetByIdAsync(userId, options);
        }
    }
}
