using System.ComponentModel.DataAnnotations;

namespace ec_project_api.Dtos.request.purchaseorders
{
    public class PurchaseOrderCreateRequest
    {
        [Required]
        public int SupplierId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required]
        public List<PurchaseOrderItemCreateRequest> Items { get; set; } = new();
    }
}
