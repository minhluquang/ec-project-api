using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ec_project_api.Models
{
    public class ProductVariant
    {
        [Key]
        public int ProductVariantId { get; set; }

        [Required]
        public int ProductId { get; set; }

        public short? ColorId { get; set; }

        public byte? SizeId { get; set; }

        [Required]
        [StringLength(50)]
        public required string Sku { get; set; }

        [Required]
        public int StockQuantity { get; set; } = 0;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }

        [ForeignKey("ColorId")]
        public virtual Color? Color { get; set; }

        [ForeignKey("SizeId")]
        public virtual Size? Size { get; set; }

        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public virtual ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}