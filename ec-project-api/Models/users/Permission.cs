using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ec_project_api.Models
{
    public class Permission
    {
        [Key]
        [Column("permission_id")]
        public short PermissionId { get; set; }
    
        [Required]
        [StringLength(100)]
        [Column("permission_name")]
        public required string PermissionName { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Required]
        [Column("resource_id")]
        [ForeignKey(nameof(Resource))]
        public short ResourceId { get; set; }
        public virtual Resource Resource { get; set; } = null!;

        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}