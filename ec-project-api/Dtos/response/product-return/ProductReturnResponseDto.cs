using ec_project_api.Dtos.response.orders;
using ec_project_api.Models;
namespace ec_project_api.Dtos.response.productReturns
{
    public class ProductReturnResponseDto
    {
        public int ReturnId { get; set; }
        public int OrderItemId { get; set; }
        public int ReturnType { get; set; }                // 1 = Đổi hàng, 2 = Hoàn tiền
        public string? ReturnReason { get; set; }
        public decimal? ReturnAmount { get; set; }
        public int? ReturnProductVariantId { get; set; }
        public int StatusId { get; set; }
        public string? StatusName { get; set; }
        public string? ProductName { get; set; }           // tên sản phẩm đã mua
        public string? ReturnProductName { get; set; }     // tên sản phẩm đổi lại (nếu có)
        public int quantity { get; set; }
        public OrderDto OrderDto { get; set; }
        public UserOrderDto UserOrderDto { get; set; }
        public string? ProductImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
