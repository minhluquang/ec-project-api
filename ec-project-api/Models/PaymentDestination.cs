using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ec_project_api.Models
{
    public class PaymentDestination
    {
        [Key]
        public int DestinationId { get; set; }

        public int? PaymentMethodId { get; set; }

        [Required]
        [StringLength(50)]
        public required string Identifier { get; set; }

        [Required]
        [StringLength(100)]
        public required string AccountName { get; set; }

        public int? StatusId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("PaymentMethodId")]
        public virtual PaymentMethod? PaymentMethod { get; set; }

        [ForeignKey("StatusId")]
        public virtual Status? Status { get; set; }

        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}