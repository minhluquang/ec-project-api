namespace ec_project_api.Dtos.response.orders
{
    public class OrderDetailDto
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string? AddressInfo { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ShippingFee { get; set; }
        public bool IsFreeShip { get; set; }
        public DateTime CreatedAt { get; set; }

        public IEnumerable<OrderItemsDto> Items { get; set; } = [];
    }
}
