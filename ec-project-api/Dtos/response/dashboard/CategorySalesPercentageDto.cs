namespace ec_project_api.Dtos.response.dashboard;

public class CategorySalesPercentageDto
{
    public string CategoryName { get; set; }
    public decimal TotalSales { get; set; }
    public decimal Percentage { get; set; }
}
