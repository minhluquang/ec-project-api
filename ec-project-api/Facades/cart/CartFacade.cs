using AutoMapper;
using ec_project_api.Dtos.response.orders;
using ec_project_api.Services;
using ec_project_api.Services.cart;
using ec_project_api.Services.cart_items;

namespace ec_project_api.Facades.cart
{
    public class CartFacade
    {
        private readonly IMapper _mapper;
        private readonly ICartService _cartService;
        private readonly ICartItemService _cartItemService;
        public CartFacade(ICartService cartService, ICartItemService cartItemService, IMapper mapper) 
        {
             _mapper = mapper;
            _cartService = cartService;
            _cartItemService = cartItemService;
        }

        public async Task<CartDetailDto> GetCartWithItemsAsync(int userId)
        {
            var cart = await _cartService.GetCartWithItemsAsync(userId);
            var cartDto = _mapper.Map<CartDetailDto>(cart);
            return cartDto;
        }
    }
}
