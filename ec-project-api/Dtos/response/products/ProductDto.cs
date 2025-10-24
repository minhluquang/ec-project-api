using ec_project_api.Dtos.Statuses;

namespace ec_project_api.Dtos.response.products {
    public class ProductDto {
        public int ProductId { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public decimal BasePrice { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal SellingPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Basic refs
        public MaterialDto? Material { get; set; }
        public CategoryDto? Category { get; set; }
        public ColorDto? Color { get; set; }
        public ProductGroupDto? ProductGroup { get; set; }
        public StatusDto? Status { get; set; }
        public ProductImageDto? PrimaryImage { get; set; }
    }
    
    public class ProductDetailDto : ProductDto {
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public int SoldQuantity { get; set; }
        public Dictionary<int, int> ReviewDetails { get; set; } = new Dictionary<int, int>();
        public int HasImageCount { get; set; }
        public IEnumerable<ProductDto>? RelatedProducts { get; set; } = [];
        public IEnumerable<ProductVariantDetailDto>? ProductVariants { get; set; } = [];
        public IEnumerable<ProductImageDetailDto>? ProductImages { get; set; } = [];
    }
}
