namespace ec_project_api.Dtos.response.orders
{
    public class OrderItemsDto
    {
        public string ProductName { get; set; } = null!;
        public string Sku { get; set; } = null!;
        public string Size { get; set; } = null!;
        public short Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal SubTotal { get; set; }
    }
}
