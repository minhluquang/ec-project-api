using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ec_project_api.Models
{
    public class PaymentMethod
    {
        [Key]
        public int PaymentMethodId { get; set; }

        [Required]
        [StringLength(100)]
        public required string MethodName { get; set; }

        public int? StatusId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("StatusId")]
        public virtual Status? Status { get; set; }

        public virtual ICollection<PaymentDestination> PaymentDestinations { get; set; } = new List<PaymentDestination>();
    }
}