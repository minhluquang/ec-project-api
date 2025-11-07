namespace ec_project_api.Dtos.response.orders
{
    public class OrderItemsDto
    {
        public int OrderItemId { get; set; }
        public int ProductVariantId { get; set; }
        public string ProductName { get; set; } = null!;
        public string Sku { get; set; } = null!;
        public string ProductImage { get; set; } = null!;
        public string Size { get; set; } = null!;
        public short Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal SubTotal { get; set; }
        public IEnumerable<ReviewOrderDto> ReviewOrder { get; set; } = [];
    }
}
