namespace ec_project_api.Dtos.response.products
{
    public class ProductVariantDto
    {
        public int ProductVariantId { get; set; }
        public int ProductId { get; set; }
        public string Sku { get; set; } = null!;
        public int StockQuantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Basic refs
        public SizeDto? Size { get; set; }
        public ColorDto? Color { get; set; }
    }
}
