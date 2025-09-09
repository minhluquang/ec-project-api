using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ec_project_api.Models
{
    public class PurchaseOrderItem
    {
        [Key]
        public int PurchaseOrderItemId { get; set; }

        [Required]
        public int PurchaseOrderId { get; set; }

        [Required]
        public int ProductVariantId { get; set; }

        [Required]
        [Range(1, short.MaxValue)]
        public short Quantity { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalPrice { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("PurchaseOrderId")]
        public virtual PurchaseOrder? PurchaseOrder { get; set; }

        [ForeignKey("ProductVariantId")]
        public virtual ProductVariant? ProductVariant { get; set; }
    }
}