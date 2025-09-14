using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ec_project_api.Models
{
    public class Resource
    {
        [Key]
        [Column("resource_id")]
        public short ResourceId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("name")]
        public required string Name { get; set; }

        [StringLength(255)]
        [Column("description")]
        public string? Description { get; set; }

        public string NameUnique => Name;

        public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
    }
}