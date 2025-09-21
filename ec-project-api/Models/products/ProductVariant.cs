using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ec_project_api.Models
{
    public class ProductVariant
    {
        [Key]
        [Column("product_variant_id")]
        public int ProductVariantId { get; set; }

        [Required]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("color_id")]
        public short? ColorId { get; set; }

        [Column("size_id")]
        public byte? SizeId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("sku")]
        public required string Sku { get; set; }

        [Required]
        [Column("stock_quantity")]
        public int StockQuantity { get; set; } = 0;

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(ProductId))]
        public virtual Product? Product { get; set; }

        [ForeignKey(nameof(ColorId))]
        public virtual Color? Color { get; set; }

        [ForeignKey(nameof(SizeId))]
        public virtual Size? Size { get; set; }

        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public virtual ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual ICollection<ProductReturn> ProductReturns { get; set; } = new List<ProductReturn>();
    }
}