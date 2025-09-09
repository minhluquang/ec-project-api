using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ec_project_api.Models
{
    public class UserRoleDetail
    {
        [Key]
        [Column(Order = 1)]
        public int UserId { get; set; }

        [Key]
        [Column(Order = 2)]
        public short RoleId { get; set; }

        public int? AssignedBy { get; set; }

        [Required]
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role? Role { get; set; }

        [ForeignKey("AssignedBy")]
        public virtual User? AssignedByUser { get; set; }
    }
}