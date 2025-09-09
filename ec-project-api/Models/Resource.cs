using System.ComponentModel.DataAnnotations;
namespace ec_project_api.Models
{
    public class Resource
    {
        [Key]
        public short ResourceId { get; set; }

        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        public string? Description { get; set; }

        public string NameUnique => Name;

        public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
    }
}