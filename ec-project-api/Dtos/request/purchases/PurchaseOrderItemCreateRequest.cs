using System.ComponentModel.DataAnnotations;

namespace ec_project_api.Dtos.request.purchaseorders
{
    public class PurchaseOrderItemCreateRequest
    {
        [Required]
        public int ProductVariantId { get; set; }

        [Required]
        [Range(1, short.MaxValue)]
        public short Quantity { get; set; }

        [Required]
        [Range(1, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [Range(0, 100)]
        public decimal ProfitPercentage { get; set; }

        public bool IsPushed { get; set; } = false;
    }
}
