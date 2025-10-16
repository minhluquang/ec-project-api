using System.ComponentModel.DataAnnotations;

namespace ec_project_api.Dtos.request.payments
{
    public class PaymentDestinationUpdateRequest
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

        public short StatusId { get; set; }
    }
}
