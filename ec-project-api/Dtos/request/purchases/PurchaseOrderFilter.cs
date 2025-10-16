using System.ComponentModel.DataAnnotations;

namespace ec_project_api.Dtos.request.purchaseorders
{
    public class PurchaseOrderFilter
    {
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 10;

        public int? StatusId { get; set; }
        public int? SupplierId { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string? OrderBy { get; set; }
    }
}
