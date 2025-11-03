namespace ec_project_api.Dtos.request.orders
{
    public class UpdateCartItemDto
    {
        public int UserId { get; set; }
        public int ProductVariantId { get; set; }
        public short Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
