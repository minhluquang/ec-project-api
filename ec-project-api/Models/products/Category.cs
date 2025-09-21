using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ec_project_api.Models
{
    public class Category
    {
        [Key]
        [Column("category_id")]
        public short CategoryId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("name")]
        public required string Name { get; set; }

        [Required]
        [StringLength(100)]
        [Column("slug")]
        public required string Slug { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("size_detail")]
        public string? SizeDetail { get; set; }

        [Column("status_id")]
        public int StatusId { get; set; }

        [Column("parent_id")]
        public short? ParentId { get; set; }

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(StatusId))]
        public virtual Status Status { get; set; } = null!;

        [ForeignKey(nameof(ParentId))]
        public virtual Category? Parent { get; set; }

        public virtual ICollection<Category> Children { get; set; } = new List<Category>();
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}