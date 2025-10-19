public class DiscountUpdateRequest
{
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DiscountType { get; set; } = string.Empty;
    public decimal DiscountValue { get; set; }
    public decimal MinOrderAmount { get; set; } = 0.00m;
    public decimal? MaxDiscountAmount { get; set; }
    public int? UsageLimit { get; set; }
    public DateTime? StartAt { get; set; }
    public DateTime? EndAt { get; set; }
    public short StatusId { get; set; }
}