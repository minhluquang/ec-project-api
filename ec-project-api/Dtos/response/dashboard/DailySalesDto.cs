namespace ec_project_api.Dtos.response.dashboard;

public class DailySalesDto
{
    public DateTime Date { get; set; }
    public string DayOfWeek { get; set; } // "Thứ 2", "Thứ 3", ...
    public decimal Revenue { get; set; }
    public int OrderCount { get; set; }
    public int ProductsSold { get; set; }
}
