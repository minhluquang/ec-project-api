using ec_project_api.Dto.response.orders;
public interface IOrderService
{
    Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
}