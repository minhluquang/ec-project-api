using ec_project_api.Dtos.response.products;

namespace ec_project_api.Dtos.response.orders
{
    public class CartDetailDto
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public List<CartItemDetailDto> CartItems { get; set; } = new List<CartItemDetailDto>();
    }

    public class CartItemDetailDto
    {
        public int CartItemId { get; set; }
        public int ProductVariantId { get; set; }
        public short Quantity { get; set; }
        public decimal Price { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImageUrl { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
    }

}
