using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ec_project_api.Models
{
    public class Size
    {
        [Key]
        [Column("size_id")]
        public byte SizeId { get; set; }

        [Required]
        [StringLength(10)]
        [Column("name")]
        public required string Name { get; set; }

        [Column("status_id")]
        [ForeignKey(nameof(Status))]
        public int StatusId { get; set; }

        public virtual Status Status { get; set; } = null!;


        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
    }
}