using System.ComponentModel.DataAnnotations;

namespace ec_project_api.Dtos.request.purchaseorders
{
    public class PurchaseOrderFilter
    {
        public string? Search { get; set; }
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 10;

        public int? StatusId { get; set; }
        public int? SupplierId { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }

        public string? OrderBy { get; set; }
    }
}
