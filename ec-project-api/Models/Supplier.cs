using System.ComponentModel.DataAnnotations;
namespace ec_project_api.Models
{
    public class Supplier
    {
        [Key]
        public int SupplierId { get; set; }

        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [Required]
        [StringLength(100)]
        public required string ContactName { get; set; }

        [Required]
        [StringLength(100)]
        public required string Email { get; set; }

        [Required]
        [StringLength(15)]
        public required string Phone { get; set; }

        [Required]
        [StringLength(255)]
        public required string Address { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
    }
}