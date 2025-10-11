namespace ec_project_api.Dtos.response.dashboard
{
    public class MonthlyRevenueDto
    {
        public string Period { get; set; } = string.Empty; // "T1", "T2", etc or "Ngày 1", "Ngày 2"
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
    }

    public class MonthlyRevenueResponse
    {
        public string TimeRange { get; set; } = string.Empty; // "current_month", "last_month", "last_3_months", "current_year"
        public List<MonthlyRevenueDto> Data { get; set; } = new();
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
    }
}

