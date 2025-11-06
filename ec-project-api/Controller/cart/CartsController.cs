using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.Controllers.Base;
using ec_project_api.Dtos.request.orders;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.orders;
using ec_project_api.Facades.cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ec_project_api.Controllers
{                                   
    [ApiController]
    [AllowAnonymous]
    [Route(PathVariables.CartRoot)]
    public class CartController : BaseController
    {
        private readonly CartFacade _cartFacade;

        public CartController(CartFacade cartFacade)
        {
            _cartFacade = cartFacade;
        }

        /// <summary>
        /// Lấy giỏ hàng của user theo UserId
        /// </summary>
        [HttpGet(PathVariables.GetCartByUserId)]
        public async Task<ActionResult<ResponseData<CartDetailDto>>> GetCartByUserId(int userId)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _cartFacade.GetCartWithItemsAsync(userId);
                return ResponseData<CartDetailDto>.Success(StatusCodes.Status200OK, result, "Lấy giỏ hàng thành công.");
            });
        }

        /// <summary>
        /// Thêm hoặc cập nhật sản phẩm trong giỏ hàng
        /// </summary>
        [HttpPost(PathVariables.UpdateCartItem)]
        public async Task<ActionResult<ResponseData<bool>>> CreateOrUpdateCartItem([FromBody] CartUpdateRequest request)
        {
            return await ExecuteAsync(async () =>
            {
                if (request.Quantity < 1)
                    throw new ArgumentException("Số lượng phải lớn hơn 0.");

                if (string.IsNullOrWhiteSpace(request.Slug))
                    throw new ArgumentException("Slug không được để trống.");

                var success = await _cartFacade.CreateOrUpdateCartItemAsync(request);
                
                if (!success)
                    throw new InvalidOperationException("Cập nhật giỏ hàng thất bại.");

                return ResponseData<bool>.Success(StatusCodes.Status200OK, true, "Cập nhật giỏ hàng thành công.");
            });
        }

        /// <summary>
        /// Xóa sản phẩm khỏi giỏ hàng
        /// </summary>
        [HttpDelete(PathVariables.DeleteCartItem)]                  
        public async Task<ActionResult<ResponseData<bool>>> DeleteCartItem(int userId, int variantId)
        {
            return await ExecuteAsync(async () =>
            {
                var success = await _cartFacade.RemoveCartItemAsync(userId, variantId);
                
                if (!success)
                    throw new KeyNotFoundException("Không tìm thấy sản phẩm trong giỏ hàng.");

                return ResponseData<bool>.Success(StatusCodes.Status200OK, true, "Xóa sản phẩm khỏi giỏ hàng thành công.");
            });
        }

        /// <summary>
        /// Xóa toàn bộ sản phẩm trong giỏ hàng của user
        /// </summary>
        [HttpDelete(PathVariables.ClearCart)]
        public async Task<ActionResult<ResponseData<bool>>> ClearCart(int userId)
        {
            return await ExecuteAsync(async () =>
            {
                var success = await _cartFacade.ClearCartAsync(userId);
                
                if (!success)
                    throw new KeyNotFoundException("Không tìm thấy giỏ hàng hoặc giỏ hàng đã trống.");

                return ResponseData<bool>.Success(StatusCodes.Status200OK, true, "Đã xóa toàn bộ sản phẩm trong giỏ hàng.");
            });
        }
    }
}
