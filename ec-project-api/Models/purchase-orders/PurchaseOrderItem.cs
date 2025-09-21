using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ec_project_api.Models
{
    public class PurchaseOrderItem
    {
        [Key]
        [Column("purchase_order_item_id")]
        public int PurchaseOrderItemId { get; set; }

        [Required]
        [Column("purchase_order_id")]
        public int PurchaseOrderId { get; set; }

        [Required]
        [Column("product_variant_id")]
        public int ProductVariantId { get; set; }

        [Required]
        [Range(1, short.MaxValue)]
        [Column("quantity")]
        public short Quantity { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        [Column("unit_price", TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [NotMapped]
        public decimal TotalPrice => Quantity * UnitPrice;

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("PurchaseOrderId")]
        public virtual PurchaseOrder? PurchaseOrder { get; set; }

        [ForeignKey("ProductVariantId")]
        public virtual ProductVariant? ProductVariant { get; set; }
    }
}