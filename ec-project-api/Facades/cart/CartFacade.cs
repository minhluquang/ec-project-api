using AutoMapper;
using ec_project_api.Dtos.request.orders;
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

        public async Task<bool> CreateOrUpdateCartItemAsync(CartUpdateRequest request)
        {
            return await _cartItemService.CreateOrUpdateCartItemAsync(request);
        }

        public async Task<bool> RemoveCartItemAsync(int userId, int variantId)
        {
            return await _cartItemService.RemoveCartItemAsync(userId, variantId);
        }

        /// <summary>
        /// Xóa toàn bộ sản phẩm trong giỏ hàng của user
        /// </summary>
        public async Task<bool> ClearCartAsync(int userId)
        {
            return await _cartItemService.ClearCartAsync(userId);
        }
    }
}
