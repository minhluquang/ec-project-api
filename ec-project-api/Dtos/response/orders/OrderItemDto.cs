using ec_project_api.Dtos.response.products;

namespace ec_project_api.Dtos.response.orders {
    public class OrderItemDto {
        public int OrderItemId { get; set; }
        public ProductVariantDto? ProductVariant { get; set; }
    }

    public class OrderItemDetailDto : OrderItemDto {
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal SubPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
