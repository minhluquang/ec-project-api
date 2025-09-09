using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ec_project_api.Models
{
    public class Color
    {
        [Key]
        public short ColorId { get; set; }

        [Required]
        [StringLength(50)]
        public required string Name { get; set; }

        [StringLength(7)]
        public string? HexCode { get; set; }

        public string? Description { get; set; }

        public int? StatusId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public string NameUnique => Name;

        [ForeignKey("StatusId")]
        public virtual Status? Status { get; set; }

        public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
    }
}