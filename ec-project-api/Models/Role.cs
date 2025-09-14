using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ec_project_api.Models
{
    public class Role
    {
        [Key]
        [Column("role_id")]
        public short RoleId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("name")]
        public required string Name { get; set; }

        [StringLength(255)]
        [Column("description")]
        public string? Description { get; set; }

        [Column("status_id")]
        [ForeignKey(nameof(Status))]
        public int StatusId { get; set; }
        public virtual Status Status { get; set; } = null!;


        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
        public virtual ICollection<UserRoleDetail> UserRoleDetails { get; set; } = new List<UserRoleDetail>();
    }
}