using ec_project_api.Constants.variables;
using ec_project_api.Dtos.request.orders;
using ec_project_api.Models;
using ec_project_api.Services;
using ec_project_api.Services.cart;
using ec_project_api.Services.cart_items;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ec_project_api.Controllers
{
    [ApiController]
    [Route(PathVariables.CartRoot)]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ICartItemService _cartItemService;

        public CartController(ICartService cartService, ICartItemService cartItemService)
        {
            _cartService = cartService;
            _cartItemService = cartItemService;
        }

        [HttpGet(PathVariables.GetCartByUserId)]
        public async Task<IActionResult> GetCartByUserId(int userId)
        {
            var cart = await _cartService.GetCartWithItemsAsync(userId);
            if (cart == null)
                return NotFound(new { message = "Giỏ hàng trống" });

            return Ok(new
            {
                items = cart.CartItems.Select(i => new
                {
                    i.CartItemId,
                    i.ProductVariantId,
                    i.Quantity,
                    i.Price,
                    i.ProductVariant
                })
            });
        }

        [HttpPost(PathVariables.UpdateCartItem)]
        public async Task<IActionResult> CreateOrUpdateCartItem([FromBody] CartUpdateRequest request)
        {
            if (request.Quantity < 1)
                return BadRequest(new { message = "Số lượng không hợp lệ" });

            var success = await _cartItemService.CreateOrUpdateCartItemAsync(request);
            if (!success)
                return StatusCode(500, new { message = "Cập nhật giỏ hàng thất bại" });

            return Ok(new { message = "Cập nhật giỏ hàng thành công" });
        }

  
        [HttpDelete(PathVariables.DeleteCartItem)]
        public async Task<IActionResult> DeleteCartItem(int userId, int variantId)
        {
            var success = await _cartItemService.RemoveCartItemAsync(userId, variantId);
            if (!success)
                return NotFound(new { message = "Không tìm thấy sản phẩm trong giỏ" });

            return Ok(new { message = "Đã xoá sản phẩm khỏi giỏ hàng" });
        }
    }

   
}
