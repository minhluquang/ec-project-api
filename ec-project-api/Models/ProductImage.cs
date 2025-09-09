using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ec_project_api.Models
{
    public class ProductImage
    {
        [Key]
        public int ProductImageId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [StringLength(255)]
        public required string ImageUrl { get; set; }

        [StringLength(255)]
        public string? AltText { get; set; }

        [Required]
        public bool IsPrimary { get; set; } = false;

        public byte? DisplayOrder { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
    }
}