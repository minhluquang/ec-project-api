using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ec_project_api.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public int AddressId { get; set; }

        [Required]
        public int UserId { get; set; }

        public int? DiscountId { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal DiscountAmount { get; set; } = 0.00m;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        [Required]
        public bool IsFreeShip { get; set; } = false;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal ShippingFee { get; set; } = 0.00m;

        public DateTime? ShippedAt { get; set; }

        public DateTime? DeliveryAt { get; set; }

        [Required]
        public int StatusId { get; set; }

        public byte? ShipId { get; set; }

        [Required]
        public int PaymentId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("AddressId")]
        public virtual Address? Address { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("DiscountId")]
        public virtual Discount? Discount { get; set; }

        [ForeignKey("StatusId")]
        public virtual Status? Status { get; set; }

        [ForeignKey("ShipId")]
        public virtual Ship? Ship { get; set; }

        [ForeignKey("PaymentId")]
        public virtual Payment? Payment { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}