namespace ec_project_api.Dtos.response.orders {
    public class OrderDto {
        public int OrderId { get; set; }
        public string? AddressInfo { get; set; }
        public int UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsFreeShip { get; set; }
        public decimal ShippingFee { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
