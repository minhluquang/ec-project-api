using ec_project_api.Models;

namespace ec_project_api.Dtos.response.dashboard;

public class TopSellingProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public string? ProductImage { get; set; }
    public string? CategoryLv2Name { get; set; }
    public int TotalQuantitySold { get; set; }
    public decimal TotalRevenue { get; set; }
}
