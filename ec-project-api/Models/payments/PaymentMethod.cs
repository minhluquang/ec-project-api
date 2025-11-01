using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ec_project_api.Models
{
    public enum PaymentMethodType
    {
        COD,
        BankTransfer,
        EWallet,
    }

    public class PaymentMethod
    {
        [Key]
        [Column("payment_method_id")]
        public int PaymentMethodId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("method_name")]
        public required string MethodName { get; set; }
        
        [Required]
        [Column("method_type")]
        public PaymentMethodType MethodType { get; set; }
        
        [Column("status_id")]
        public short StatusId { get; set; }

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("StatusId")]
        public virtual Status Status { get; set; } = null!;

        public virtual ICollection<PaymentDestination> PaymentDestinations { get; set; } = new List<PaymentDestination>();
    }
}