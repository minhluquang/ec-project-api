using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ec_project_api.Models {
    public class Material {
        [Key]
        [Column("material_id")]
        public short MaterialId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("name")]
        public required string Name { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("status_id")]
        public short StatusId { get; set; }

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(StatusId))]
        public virtual Status Status { get; set; } = null!;

        public virtual ICollection<Product> Products { get; set; } = [];
    }
}