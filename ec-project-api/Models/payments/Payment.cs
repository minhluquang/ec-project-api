using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ec_project_api.Models
{
    public class Payment
    {
        [Key]
        [Column("payment_id")]
        public int PaymentId { get; set; }

        [Column("destination_id")]
        public int? DestinationId { get; set; }

        [Column("status_id")]
        public short StatusId { get; set; }

        [Required]
        [Column("amount", TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [StringLength(100)]
        [Column("transaction_id")]
        public string? TransactionId { get; set; }

        [Column("paid_at")]
        public DateTime? PaidAt { get; set; }

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("DestinationId")]
        public virtual PaymentDestination? PaymentDestination { get; set; }

        [ForeignKey("StatusId")]
        public virtual Status Status { get; set; } = null!;

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}