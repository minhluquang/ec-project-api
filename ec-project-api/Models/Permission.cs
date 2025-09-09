using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ec_project_api.Models
{
    public class Permission
    {
        [Key]
        public short PermissionId { get; set; }

        [Required]
        [StringLength(100)]
        public required string PermissionName { get; set; }

        public string? Description { get; set; }

        public int? StatusId { get; set; }

        public short? ResourceId { get; set; }

        public string PermissionNameUnique => PermissionName;

        [ForeignKey("StatusId")]
        public virtual Status? Status { get; set; }

        [ForeignKey("ResourceId")]
        public virtual Resource? Resource { get; set; }

        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}