namespace ec_project_api.Dtos.response.dashboard

{
    public class DashboardOverviewDto
    {
        public decimal MonthlyRevenue { get; set; }
        public decimal RevenueChangePercent { get; set; }
        public int TotalOrders { get; set; }
        public decimal OrderChangePercent { get; set; }
        public int NewCustomers { get; set; }
        public decimal CustomerChangePercent { get; set; }
        public int ProductsSold { get; set; }
        public decimal ProductChangePercent { get; set; }
    }
}