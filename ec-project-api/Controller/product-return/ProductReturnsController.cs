using ec_project_api.Constants.messages;
using ec_project_api.Constants.variables;
using ec_project_api.Controllers.Base;
using ec_project_api.Dtos.request.product_return;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.pagination;
using ec_project_api.Dtos.response.productReturns;
using ec_project_api.Facades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ec_project_api.Controller.product_return
{
    [Route(PathVariables.ProductReturnRoot)]
    [ApiController]
    [AllowAnonymous]                                        
    public class ProductReturnController : BaseController
    {
        private readonly ProductReturnFacade _productReturnFacade;

        public ProductReturnController(ProductReturnFacade productReturnFacade)
        {
            _productReturnFacade = productReturnFacade;
        }

        /// <summary>
        /// Lấy danh sách tất cả phiếu đổi trả hàng
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ResponseData<PagedResult<ProductReturnResponseDto>>>> GetAllProductReturns(
            [FromQuery] ProductReturnFilter filter)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _productReturnFacade.GetAllPagedAsync(filter);
                return ResponseData<PagedResult<ProductReturnResponseDto>>.Success(
                    StatusCodes.Status200OK, 
                    result, 
                    ProductReturnMessages.ProductReturnsRetrievedSuccessfully);
            });
        }

        /// <summary>
        /// Tạo phiếu đổi trả hàng
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ResponseData<ProductReturnResponseDto>>> CreateProductReturn([FromBody] CreateProductReturnDto dto)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _productReturnFacade.CreateProductReturnAsync(dto);
                return ResponseData<ProductReturnResponseDto>.Success(
                    StatusCodes.Status201Created, 
                    result, 
                    ProductReturnMessages.SuccessfullyCreatedProductReturn);
            });
        }

        /// <summary>
        /// Xóa phiếu đổi trả hàng (chỉ cho phép khi trạng thái là Draft)
        /// </summary>
        [HttpDelete(PathVariables.Delete)]
        public async Task<ActionResult<ResponseData<bool>>> DeleteProductReturn(int id)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _productReturnFacade.DeleteProductReturnAsync(id);
                return ResponseData<bool>.Success(
                    StatusCodes.Status200OK,
                    result,
                    ProductReturnMessages.SuccessfullyDeletedProductReturn);
            });
        }

        /// <summary>
        /// Duyệt phiếu đổi trả hàng
        /// </summary>
        [HttpPut(PathVariables.ApproveReturn)]
        public async Task<ActionResult<ResponseData<bool>>> ApproveProductReturn(int returnId)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _productReturnFacade.ApproveProductReturnAsync(returnId);
                return ResponseData<bool>.Success(
                    StatusCodes.Status200OK,
                    result,
                    ProductReturnMessages.SuccessfullyApprovedProductReturn);
            });
        }

        /// <summary>
        /// Từ chối phiếu đổi trả hàng
        /// </summary>
        [HttpPut(PathVariables.RejectedReturn)]
        public async Task<ActionResult<ResponseData<bool>>> RejectedProductReturn(int returnId)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _productReturnFacade.RejectedProductReturnAsync(returnId);
                return ResponseData<bool>.Success(
                    StatusCodes.Status200OK,
                    result,
                    ProductReturnMessages.SuccessfullyRejectedProductReturn);
            });
        }

      
        [HttpPut(PathVariables.CompleteReturnForRefund)]
        public async Task<ActionResult<ResponseData<bool>>> CompleteReturnForRefund(int returnId)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _productReturnFacade.CompleteProductReturnForReturnAsync(returnId);
                return ResponseData<bool>.Success(
                    StatusCodes.Status200OK,
                    result,
                    ProductReturnMessages.SuccessfullyCompletedReturnForRefund);
            });
        }

        [HttpPut(PathVariables.CompleteReturnForExchange)]
        public async Task<ActionResult<ResponseData<bool>>> CompleteReturnForExchange(int returnId)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _productReturnFacade.CompleteProductReturnForExchangeAsync(returnId);
                return ResponseData<bool>.Success(
                    StatusCodes.Status200OK,
                    result,
                    ProductReturnMessages.SuccessfullyCompletedReturnForExchange);
            });
        }
    }
}