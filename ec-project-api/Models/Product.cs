using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ec_project_api.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [StringLength(255)]
        public required string Name { get; set; }

        [Required]
        [StringLength(255)]
        public required string Slug { get; set; }

        [Required]
        public short MaterialId { get; set; }

        [Required]
        public short CategoryId { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal BasePrice { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? SalePrice { get; set; }

        [Range(0, 100)]
        public decimal? DiscountPercentage { get; set; }

        public int? StatusId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("MaterialId")]
        public virtual Material? Material { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        [ForeignKey("StatusId")]
        public virtual Status? Status { get; set; }

        public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
        public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}