using ec_project_api.Constants.variables;
using ec_project_api.Dtos.request.orders;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.orders;
using ec_project_api.Facades.cart;
using ec_project_api.Models;
using ec_project_api.Services;
using ec_project_api.Services.cart;
using ec_project_api.Services.cart_items;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ec_project_api.Controllers
{                                   
    [ApiController]
    [AllowAnonymous]
    [Route(PathVariables.CartRoot)]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ICartItemService _cartItemService;
        private readonly CartFacade _cartFacade;

        public CartController(ICartService cartService, ICartItemService cartItemService, CartFacade cartFacade)
        {
            _cartService = cartService;
            _cartItemService = cartItemService;
            _cartFacade = cartFacade;
        }

        [HttpGet(PathVariables.GetCartByUserId)]
        public async Task<ActionResult<ResponseData<CartDetailDto>>> GetCartByUserId(int userId)
        {
            try
            {
                var result = await _cartFacade.GetCartWithItemsAsync(userId);
                return Ok(ResponseData<CartDetailDto>.Success(200, result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseData<CartDetailDto>.Error(500, ex.Message));
            }
        }

        [HttpPost(PathVariables.UpdateCartItem)]
        public async Task<ActionResult<ResponseData<bool>>> CreateOrUpdateCartItem([FromBody] CartUpdateRequest request)
        {
            if (request.Quantity < 1)
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, "Số lượng không hợp lệ."));

            var success = await _cartItemService.CreateOrUpdateCartItemAsync(request);
            if (!success)
                return StatusCode(500, ResponseData<bool>.Error(500, "Cập nhật giỏ hàng thất bại"));

            return Ok(ResponseData<bool>.Success(200, true));
        }

  
        [HttpDelete(PathVariables.DeleteCartItem)]                  
        public async Task<IActionResult> DeleteCartItem(int userId, int variantId)
        {
            var success = await _cartItemService.RemoveCartItemAsync(userId, variantId);
            if (!success)
                return NotFound(ResponseData<bool>.Error(404, "Không tìm thấy sản phẩm trong giỏ"));

            return Ok(ResponseData<bool>.Success(200, true));
        }
    }

   
}
