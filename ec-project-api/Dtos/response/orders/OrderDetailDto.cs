using ec_project_api.Models;

namespace ec_project_api.Dtos.response.orders
{
    public class UserOrderDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Phone { get; set; }
    }

    public class StatusOrderDto
    {
        public short StatusId { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class ShipOrderDto
    {
        public short ShipId { get; set; }
        public string CorpName { get; set; }
    }
    public class OrderDetailDto
    {
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsFreeShip { get; set; }
        public decimal ShippingFee { get; set; }
        public string? AddressInfo { get; set; }
        public UserOrderDto User { get; set; } = null!;
        public ShipOrderDto Ship {  get; set; } = null!;
        public StatusOrderDto Status { get; set; } = null!;

        public IEnumerable<OrderItemsDto> Items { get; set; } = [];
    }

}
