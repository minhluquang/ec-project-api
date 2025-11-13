using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ec_project_api.Models
{
    public class Order
    {
        [Key]
        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("address_info")]
        public string? AddressInfo { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("discount_id")]
        public int? DiscountId { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        [Column("total_amount", TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        [Column("is_free_ship")]
        public bool IsFreeShip { get; set; } = false;

        [Required]
        [Column("received_name")]
        public string? ReceivedName { get; set; } = string.Empty;

        [Required]
        [Column("received_phone")]
        public string? ReceivedPhone { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        [Column("shipping_fee", TypeName = "decimal(18,2)")]
        public decimal ShippingFee { get; set; } = 0.00m;

        [Column("shipped_at")]
        public DateTime? ShippedAt { get; set; }

        [Column("delivery_at")]
        public DateTime? DeliveryAt { get; set; }

        [Column("status_id")]
        public short StatusId { get; set; }

        [Column("ship_id")]
        public short? ShipId { get; set; }

        [Column("payment_id")]
        public int? PaymentId { get; set; }

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(UserId))]
        public virtual User? User { get; set; }

        [ForeignKey(nameof(DiscountId))]
        public virtual Discount? Discount { get; set; }

        [ForeignKey(nameof(StatusId))]
        public virtual Status Status { get; set; } = null!;

        [ForeignKey(nameof(ShipId))]
        public virtual Ship? Ship { get; set; }

        [ForeignKey(nameof(PaymentId))]
        public virtual Payment? Payment { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}