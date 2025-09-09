using System.ComponentModel.DataAnnotations;
namespace ec_project_api.Models
{
    public class Size
    {
        [Key]
        public byte SizeId { get; set; }

        [Required]
        [StringLength(10)]
        public required string Name { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
    }
}