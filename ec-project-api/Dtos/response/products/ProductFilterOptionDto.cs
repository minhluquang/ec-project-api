namespace ec_project_api.Dtos.response.products;

public class ProductFilterOptionDto
{
    public IEnumerable<ColorStatDto> ColorOptions { get; set; } = [];
    public IEnumerable<MaterialStatDto> MaterialOptions {get; set; } = [];
    public IEnumerable<ProductGroupStatDto> ProductGroupOptions { get; set; } = [];
    public IEnumerable<StockStatusDto> StockStatusOptions { get; set; } = [];
}