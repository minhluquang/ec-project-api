using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.Controllers.Base;
using ec_project_api.Dtos.order;
using ec_project_api.Dtos.request.orders;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.orders;
using ec_project_api.Dtos.response.pagination;
using ec_project_api.Facades.orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ec_project_api.Controllers.orders
{
    [Route(PathVariables.OrderRoot)]
    [ApiController]
    public class OrderController : BaseController
    {
        private readonly OrderFacade _orderFacade;

        public OrderController(OrderFacade orderFacade)
        {
            _orderFacade = orderFacade;
        }

        [HttpGet]
        [Authorize(Policy = "Order.GetAll")]
        public async Task<ActionResult<ResponseData<PagedResult<OrderDetailDto>>>> GetAll([FromQuery] OrderFilter filter)
        {
            return await ExecuteAsync(async () =>
            {
                var orders = await _orderFacade.GetAllPagedAsync(filter);
                return ResponseData<PagedResult<OrderDetailDto>>.Success(StatusCodes.Status200OK, orders, OrderMessages.OrdersRetrievedSuccessfully);
            });
        }

        [HttpGet(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<OrderDetailDto>>> GetById(int id)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _orderFacade.GetOrderByIdAsync(id);
                return ResponseData<OrderDetailDto>.Success(StatusCodes.Status200OK, result, OrderMessages.OrderRetrievedSuccessfully);
            });
        }

        [HttpPost]
        public async Task<ActionResult<ResponseData<OrderDetailDto>>> Create([FromBody] OrderCreateRequest request)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _orderFacade.CreateOrderAsync(request);
                return ResponseData<OrderDetailDto>.Success(StatusCodes.Status201Created, result, OrderMessages.SuccessfullyCreatedOrder);
            });
        }

        [HttpPatch(PathVariables.GetById)]
        [Authorize(Policy = "Order.UpdateStatus")]
        public async Task<ActionResult<ResponseData<bool>>> UpdateStatus(int id, [FromBody] UpdateOrderStatusRequest request)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _orderFacade.UpdateOrderStatusAsync(id, request.NewStatusId);
                return ResponseData<bool>.Success(StatusCodes.Status200OK, result, OrderMessages.SuccessfullyUpdatedOrder);
            });
        }

        [HttpPut(PathVariables.ApproveOrder)]
        [Authorize(Policy = "Order.Approve")]
        public async Task<ActionResult<ResponseData<bool>>> UpdateNextStatus(int orderId)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _orderFacade.AutoUpdateNextStatusAsync(orderId);
                return ResponseData<bool>.Success(StatusCodes.Status200OK, result, OrderMessages.OrderStatusChanged);
            });
        }

        [HttpPut(PathVariables.CancelOrder)]
        public async Task<ActionResult<ResponseData<bool>>> CancelOrder(int orderId)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _orderFacade.CancelOrderAsync(orderId);
                return ResponseData<bool>.Success(StatusCodes.Status200OK, result, OrderMessages.OrderCancelledSuccessfully);
            });
        }

        [HttpPut(PathVariables.CompleteOrder)]
        public async Task<ActionResult<ResponseData<bool>>> ConfirmReceivedlOrder(int orderId)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _orderFacade.CompleteOrderAsync(orderId);
                return ResponseData<bool>.Success(StatusCodes.Status200OK, result, OrderMessages.OrderCompletedSuccessfully);
            });
        }

        [HttpGet(PathVariables.OrderUserId)]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseData<IEnumerable<OrderDetailDto>>>> GetOrdersByUserId(int userId)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _orderFacade.GetOrdersByUserIdAsync(userId);
                return ResponseData<IEnumerable<OrderDetailDto>>.Success(StatusCodes.Status200OK, result, OrderMessages.OrdersRetrievedSuccessfully);
            });
        }

    }
}
