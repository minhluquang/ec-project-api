using ec_project_api.Constants.messages;
using ec_project_api.Constants.variables;
using ec_project_api.Dtos.request.product_return;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.product_return;
using ec_project_api.Dtos.response.productReturns;
using ec_project_api.Dtos.response.products;
using ec_project_api.Facades;
using Microsoft.AspNetCore.Mvc;

namespace ec_project_api.Controller.product_return
{
    [Route(PathVariables.ProductReturnRoot)]
    [ApiController]
    public class ProductReturnController : ControllerBase
    {
        private readonly ProductReturnFacade _productReturnFacade;

        public ProductReturnController(ProductReturnFacade productReturnFacade)
        {
            _productReturnFacade = productReturnFacade;
        }

        /// <summary>
        /// Tạo phiếu đổi trả hàng
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ProductReturnResponseDto>> CreateProductReturn([FromBody] CreateProductReturnDto dto)
        {
            try
            {
                var result = await _productReturnFacade.CreateProductReturnAsync(dto);
                return Ok(ResponseData<ProductReturnResponseDto>.Success(StatusCodes.Status200OK, result));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<ProductReturnResponseDto>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        /// <summary>
        /// Xóa phiếu đổi trả hàng (chỉ cho phép khi trạng thái là Draft)
        /// </summary>
        [HttpDelete(PathVariables.Delete)]
        public async Task<ActionResult<bool>> DeleteProductReturn(int returnId)
        {
            try
            {
                var result = await _productReturnFacade.DeleteProductReturnAsync(returnId);
                return Ok(ResponseData<bool>.Success(
                    StatusCodes.Status200OK,
                    result,
                    ProductReturnMessages.SuccessfullyDeletedProductReturn
                ));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ResponseData<bool>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }
    }
}