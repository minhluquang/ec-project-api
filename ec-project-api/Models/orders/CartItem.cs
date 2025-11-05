using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ec_project_api.Models
{
    public class CartItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("cart_item_id")]
        public int CartItemId { get; set; }

        [Required]
        [Column("cart_id")]
        public int CartId { get; set; }

        [Required]
        [Column("product_variant_id")]
        public int ProductVariantId { get; set; }

        [Required]
        [Range(1, short.MaxValue)]
        [Column("quantity")]
        public short Quantity { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        [Column("price")]
        public decimal Price { get; set; }

        [Required]
        [StringLength(255)]
        [Column("slug")]
        public string Slug { get; set; } = string.Empty;

        [NotMapped]
        public string CombinedUnique => $"{CartId}_{ProductVariantId}";

        [ForeignKey(nameof(CartId))]
        public virtual Cart? Cart { get; set; }

        [ForeignKey(nameof(ProductVariantId))]
        public virtual ProductVariant? ProductVariant { get; set; }
    }
}