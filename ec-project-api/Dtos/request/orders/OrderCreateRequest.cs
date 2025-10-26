namespace ec_project_api.Dtos.request.orders
{
    public class OrderCreateRequest
    {
        public int UserId { get; set; }
        public byte? DiscountId { get; set; }
        public byte? ShipId { get; set; } // thêm ShipId
        public string? AddressInfo { get; set; }
        public bool IsFreeShip { get; set; } = false;
        public decimal ShippingFee { get; set; } = 0.00m;
        public List<OrderItemCreateRequest> Items { get; set; } = new();
    }

}
