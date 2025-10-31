namespace ec_project_api.Dtos.response.products;

public class StockStatusDto
{
    public string Label { get; set; } = null!;
    public bool InStock { get; set; }
    public int ProductCount { get; set; }
}