using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ec_project_api.Models {
    public class ProductVariant {
        [Key]
        [Column("product_variant_id")]
        public int ProductVariantId { get; set; }

        [Required]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("size_id")]
        public byte SizeId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("sku")]
        public required string Sku { get; set; }

        [Required]
        [Column("stock_quantity")]
        public int StockQuantity { get; set; } = 0;

        [Column("status_id")]
        public short StatusId { get; set; }

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(ProductId))]
        public virtual Product Product { get; set; } = null!;

        [ForeignKey(nameof(SizeId))]
        public virtual Size? Size { get; set; }

        [ForeignKey(nameof(StatusId))]
        public virtual Status? Status { get; set; }

        public virtual ICollection<CartItem> CartItems { get; set; } = [];
        public virtual ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = [];
        public virtual ICollection<OrderItem> OrderItems { get; set; } = [];
        public virtual ICollection<ProductReturn> ProductReturns { get; set; } = [];
    }
}