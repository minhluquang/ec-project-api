using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.Dtos.order;
using ec_project_api.Dtos.request.orders;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.orders;
using ec_project_api.Facades.orders;
using Microsoft.AspNetCore.Mvc;

namespace ec_project_api.Controllers.orders
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderFacade _orderFacade;

        public OrderController(OrderFacade orderFacade)
        {
            _orderFacade = orderFacade;
        }

        /// <summary>
        /// ✅ Lấy danh sách tất cả đơn hàng (bao gồm User, Status, Ship, Payment, Discount, Items...)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ResponseData<IEnumerable<OrderDetailDto>>>> GetAllOrders()
        {
            try
            {
                var result = await _orderFacade.GetAllAsync();
                return Ok(ResponseData<IEnumerable<OrderDetailDto>>.Success(200, result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseData<IEnumerable<OrderDetailDto>>.Error(500, ex.Message));
            }
        }
        [HttpPost]
        public async Task<ActionResult<ResponseData<OrderDto>>> CreateOrder([FromBody] OrderCreateRequest request)
        {
            try
            {
                var result = await _orderFacade.CreateOrderAsync(request);
                return Ok(ResponseData<OrderDetailDto>.Success(StatusCodes.Status201Created, result));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ResponseData<OrderDto>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseData<OrderDto>.Error(StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
        [HttpPut(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<string>>> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequest request)
        {
            try
            {
                var result = await _orderFacade.UpdateOrderStatusAsync(id, request.NewStatusId);
                if (!result)
                    return BadRequest(ResponseData<string>.Error(StatusCodes.Status400BadRequest, OrderMessages.InvalidStatusTransition));

                return Ok(ResponseData<string>.Success(StatusCodes.Status200OK, OrderMessages.SuccessfullyUpdatedOrder));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ResponseData<string>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseData<string>.Error(StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
    }
}
