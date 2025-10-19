namespace ec_project_api.Dtos.request.orders
{
    public class OrderItemCreateRequest
    {
        public int ProductVariantId { get; set; }
        public short Quantity { get; set; }
    }
}
