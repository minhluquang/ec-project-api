using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ec_project_api.Models
{
    public class UserRoleDetail
    {
         [Column("user_id")]
        public int UserId { get; set; }

        [Column("role_id")]
        public short RoleId { get; set; }

        [Column("assigned_by")]
        public int? AssignedBy { get; set; }

        [Required]
        [Column("assigned_at")]
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;

        [ForeignKey(nameof(RoleId))]
        public virtual Role Role { get; set; } = null!;

        [ForeignKey(nameof(AssignedBy))]
        public virtual User? AssignedByUser { get; set; }
    }
}