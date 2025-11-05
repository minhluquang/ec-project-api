namespace ec_project_api.Dtos.response.dashboard;

public class MonthlyRevenueStatsDto
{
    public string Period { get; set; } // "T1", "T2", ...
    public decimal Revenue { get; set; }
    public int OrderCount { get; set; }
}
