using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ec_project_api.Models {
    public class ProductGroup {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("product_group_id")]
        public int ProductGroupId { get; set; }

        [Column("name")]
        public string? Name { get; set; }

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