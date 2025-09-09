using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ec_project_api.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        public int? DestinationId { get; set; }

        public int? StatusId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [StringLength(100)]
        public string? TransactionId { get; set; }

        public DateTime? PaidAt { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("DestinationId")]
        public virtual PaymentDestination? PaymentDestination { get; set; }

        [ForeignKey("StatusId")]
        public virtual Status? Status { get; set; }

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}