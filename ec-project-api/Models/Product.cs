using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ec_project_api.Models
{
    public class Product
    {
        [Key]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Required]
        [StringLength(255)]
        [Column("name")]
        public required string Name { get; set; }

        [Required]
        [StringLength(255)]
        [Column("slug")]
        public required string Slug { get; set; }

        [Required]
        [Column("material_id")]
        public short MaterialId { get; set; }

        [Required]
        [Column("category_id")]
        public short CategoryId { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        [Column("base_price", TypeName = "decimal(18,2)")]
        public decimal BasePrice { get; set; }

        [Range(0, 100)]
        [Column("discount_percentage", TypeName = "decimal(5,2)")]
        public decimal? DiscountPercentage { get; set; }

        [Column("status_id")]
        public int StatusId { get; set; }

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(MaterialId))]
        public virtual Material Material { get; set; } = null!;

        [ForeignKey(nameof(CategoryId))]
        public virtual Category Category { get; set; } = null!;

        [ForeignKey(nameof(StatusId))]
        public virtual Status Status { get; set; } = null!;

        public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
        public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    }
}