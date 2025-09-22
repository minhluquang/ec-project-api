using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ec_project_api.Models
{
    public class Color
    {
        [Key]
        [Column("color_id")]
        public short ColorId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("name")]
        public required string Name { get; set; }

        [Column("display_name")]
        public string? DisplayName { get; set; }

        [StringLength(7)]
        [Column("hex_code")]
        public string? HexCode { get; set; }

        [Column("status_id")]
        public int StatusId { get; set; }

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [Required]
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [NotMapped]
        public string NameUnique => Name;

        [ForeignKey(nameof(StatusId))]
        public virtual Status Status { get; set; } = null!;

        public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
    }
}