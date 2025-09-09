using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ec_project_api.Models
{
    public class Address
    {
        [Key]
        public int AddressId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public required string RecipientName { get; set; }

        [Required]
        [StringLength(15)]
        public required string Phone { get; set; }

        [Required]
        [StringLength(255)]
        public required string StreetAddress { get; set; }

        [Required]
        [StringLength(50)]
        public required string City { get; set; }

        [Required]
        [StringLength(50)]
        public required string Ward { get; set; }

        [Required]
        [StringLength(50)]
        public required string District { get; set; }

        [Required]
        public bool IsDefault { get; set; } = false;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}