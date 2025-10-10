using System.ComponentModel.DataAnnotations;

namespace ec_project_api.DTOs.Payments
{
    public class PaymentDestinationCreateRequest
    {
        [Required]
        public int PaymentMethodId { get; set; }

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

        [Required]
        public int StatusId { get; set; }
    }
}
