using System.ComponentModel.DataAnnotations;
namespace ec_project_api.Models
{
    public class Role
    {
        [Key]
        public short RoleId { get; set; }

        [Required]
        [StringLength(50)]
        public required string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<User> Users { get; set; } = new List<User>();
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
        public virtual ICollection<UserRoleDetail> UserRoleDetails { get; set; } = new List<UserRoleDetail>();
    }
}