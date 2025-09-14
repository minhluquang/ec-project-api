using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ec_project_api.Models
{
    public class ProductImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("product_image_id")]
        public int ProductImageId { get; set; }

        [Required]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Required]
        [StringLength(255)]
        [Column("image_url")]
        public string ImageUrl { get; set; } = string.Empty;

        [StringLength(255)]
        [Column("alt_text")]
        public string? AltText { get; set; }

        [Required]
        [Column("is_primary")]
        public bool IsPrimary { get; set; } = false;

        [Column("display_order")]
        public byte? DisplayOrder { get; set; }

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(ProductId))]
        public virtual Product? Product { get; set; }
    }
}