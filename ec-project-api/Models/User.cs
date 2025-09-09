using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ec_project_api.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        public short? RoleId { get; set; }

        [Required]
        [StringLength(50)]
        public required string Username { get; set; }

        [Required]
        [StringLength(100)]
        public required string Email { get; set; }

        [Required]
        [StringLength(32)]
        public required string Salt { get; set; }

        [Required]
        [StringLength(60)]
        public required string PasswordHash { get; set; }

        [StringLength(255)]
        public string? ImageUrl { get; set; }

        [StringLength(100)]
        public string? FullName { get; set; }

        [StringLength(15)]
        public string? Phone { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public int? StatusId { get; set; }

        [Required]
        public bool IsVerified { get; set; } = false;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("RoleId")]
        public virtual Role? Role { get; set; }

        [ForeignKey("StatusId")]
        public virtual Status? Status { get; set; }

        public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
        public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
        public virtual ICollection<UserRoleDetail> UserRoleDetails { get; set; } = new List<UserRoleDetail>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}