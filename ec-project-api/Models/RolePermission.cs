using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ec_project_api.Models
{
    public class RolePermission
    {
        [Key]
        [Column(Order = 1)]
        public short RoleId { get; set; }

        [Key]
        [Column(Order = 2)]
        public short PermissionId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("RoleId")]
        public virtual Role? Role { get; set; }

        [ForeignKey("PermissionId")]
        public virtual Permission? Permission { get; set; }
    }
}