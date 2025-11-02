namespace ec_project_api.Dtos.response.homepage
{
    public class HomepageDto
    {
        public List<CategoryHomePageDto> Categories { get; set; } = new();
        public List<ProductSummaryDto> BestSellingProducts { get; set; } = new();
        public List<ProductSummaryDto> OnSaleProducts { get; set; } = new();
        public List<CategorySalesDto> BestSellingCategories { get; set; } = new();
    }

    public class CategoryHomePageDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? Description { get; set; }
        public bool HasProduct { get; set; }
        public List<CategoryHomePageDto> Children { get; set; } = new();
    }

    public class ProductSummaryDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = null!;
        public string? Thumbnail { get; set; }
        public decimal Price { get; set; }
        public decimal? SalePrice { get; set; }
        public int SoldQuantity { get; set; }
        public int DiscountPercentage { get; set; }
        public string Slug { get; set; } = null!;
    }

    public class CategorySalesDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public int TotalSold { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
