using Microsoft.AspNetCore.Mvc;
using ec_project_api.Dto.response.orders;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<ActionResult<ResponseData<IEnumerable<OrderDto>>>> GetAllOrders()
    {
        var orders = await _orderService.GetAllOrdersAsync();

        if (orders == null || !orders.Any())
        {
            return ResponseData<IEnumerable<OrderDto>>.Error(404, "Không tìm thấy đơn hàng");
        }

        return ResponseData<IEnumerable<OrderDto>>.Success(200, orders, "Thành công");
    }
}
