using System.ComponentModel.DataAnnotations;

namespace ec_project_api.DTOs.Payments
{
    public class PaymentDestinationUpdateRequest
    {
        [Required]
        [StringLength(50)]
        public string Identifier { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string BankName { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string AccountName { get; set; } = string.Empty;

        public int? StatusId { get; set; }

        public int? PaymentMethodId { get; set; }
    }
}
