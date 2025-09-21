using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ec_project_api.Models
{
    public class RolePermission
    {
        [Column("role_id")]
        public short RoleId { get; set; }

        [Column("permission_id")]
        public short PermissionId { get; set; }

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(RoleId))]
        public virtual Role Role { get; set; } = null!;

        [ForeignKey(nameof(PermissionId))]
        public virtual Permission Permission { get; set; } = null!;
    }
}