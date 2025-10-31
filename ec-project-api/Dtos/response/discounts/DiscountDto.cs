using ec_project_api.Dtos.Statuses;

namespace ec_project_api.Dtos.response.discounts
{
    public class DiscountDto
    {
        public int DiscountId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string DiscountType { get; set; } = string.Empty;
        public decimal DiscountValue { get; set; }
        public decimal MinOrderAmount { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public int? UsageLimit { get; set; }
        public int UsedCount { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }

        // primitive status fields to avoid cycles when serializing lists
        public short StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class DiscountDetailDto : DiscountDto
    {
        // Optional full status object � ensure StatusDto contains no navigation collections
        public StatusDto? Status { get; set; }

        // additional detail fields (if needed)
        // note: do NOT include navigation collections (e.g. Orders) to prevent cycles
    }
}