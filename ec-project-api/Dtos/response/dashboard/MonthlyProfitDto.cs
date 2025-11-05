namespace ec_project_api.Dtos.response.dashboard;

public class MonthlyProfitDto
{
    public string Period { get; set; } // "T1", "T2", ...
    public decimal TotalRevenue { get; set; } // Doanh thu từ orders delivered
    public decimal TotalCost { get; set; } // Chi phí từ orderPurchase
    public decimal ShippingRevenue { get; set; } // Tổng phí ship thu được
    public decimal Profit { get; set; } // Lợi nhuận = Revenue - Cost
    public decimal ProfitMargin { get; set; } // % lợi nhuận
}
