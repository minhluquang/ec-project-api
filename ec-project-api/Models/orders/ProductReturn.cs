using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ec_project_api.Models
{
    public class ProductReturn
    {
        [Key]
        [Column("return_id")]
        public int ReturnId { get; set; }

        [Required]
        [Column("order_item_id")]
        public int OrderItemId { get; set; }

        [ForeignKey(nameof(OrderItemId))]
        public virtual OrderItem OrderItem { get; set; } = null!;

        [Required]
        [Column("return_type")]
        public int ReturnType { get; set; }

        [Column("return_reason",TypeName = "nvarchar(255)")]
        public string? ReturnReason { get; set; }

        [Column("return_amount", TypeName = "decimal(18,2)")]
        public decimal? ReturnAmount { get; set; }

        [Column("return_product_variant_id")]
        public int? ReturnProductVariantId { get; set; }

        [Required]
        [Column("quantity")]
        public int quantity { get; set; }

        [ForeignKey(nameof(ReturnProductVariantId))]
        public virtual ProductVariant? ReturnProductVariant { get; set; }

        [Column("status_id")]
        public short StatusId { get; set; }

        [ForeignKey(nameof(StatusId))]
        public virtual Status Status { get; set; } = null!;

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
