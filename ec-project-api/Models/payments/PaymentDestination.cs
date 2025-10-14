using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ec_project_api.Models
{
    public class PaymentDestination
    {
        [Key]
        [Column("destination_id")]
        public int DestinationId { get; set; }

        [Column("payment_method_id")]
        public int? PaymentMethodId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("identifier")]
        public required string Identifier { get; set; }

        [Required]
        [StringLength(100)]
        [Column("bank_name")]
        public required string BankName { get; set; }
       
        [Required]
        [StringLength(100)]
        [Column("account_name")]
        public required string AccountName { get; set; }

        [Column("status_id")]
        public int StatusId { get; set; }

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("PaymentMethodId")]
        public virtual PaymentMethod? PaymentMethod { get; set; }

        [ForeignKey("StatusId")]
        public virtual Status Status { get; set; } = null!;

        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}