using ec_project_api.Dtos.Statuses;

namespace ec_project_api.Dtos.response.discounts
{
    public class DiscountDto
    {
        public int DiscountId { get; set; }
        public string Code { get; set; } = string.Empty;
       
    }

    public class DiscountDetailDto : DiscountDto
    {
        public StatusDto? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? Description { get; set; }
        public string DiscountType { get; set; } = string.Empty;
        public decimal DiscountValue { get; set; }
        public decimal MinOrderAmount { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public int? UsageLimit { get; set; }
        public int UsedCount { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
    }
}