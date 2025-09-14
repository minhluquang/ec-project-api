using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ec_project_api.Models
{
    public class User
    {
        [Key]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("username")]
        public required string Username { get; set; }

        [Required]
        [StringLength(100)]
        [Column("email")]
        public required string Email { get; set; }

        [Required]
        [StringLength(60)]
        [Column("password_hash")]
        public required string PasswordHash { get; set; }

        [StringLength(255)]
        [Column("image_url")]
        public string? ImageUrl { get; set; }

        [StringLength(100)]
        [Column("full_name")]
        public string? FullName { get; set; }

        [StringLength(15)]
        [Column("phone")]
        public string? Phone { get; set; }

        [StringLength(10)]
        [Column("gender")]
        public string? Gender { get; set; }

        [Column("date_of_birth")]
        public DateTime? DateOfBirth { get; set; }

        [Required]
        [Column("is_verified")]
        public bool IsVerified { get; set; } = false;

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Column("status_id")]
        [ForeignKey(nameof(Status))]
        public int StatusId { get; set; }
        public virtual Status Status { get; set; } = null!;

        public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
        public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
        public virtual ICollection<UserRoleDetail> UserRoleDetails { get; set; } = new List<UserRoleDetail>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}