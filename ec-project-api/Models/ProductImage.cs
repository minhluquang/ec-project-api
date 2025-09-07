namespace ec_project_api.Models
{
    public class ProductImage
    {
        public int ProductImageId { get; set; }
        public string ImageUrl { get; set; }
        public string? AltText { get; set; }
        public bool IsPrimary { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Foreign Keys + Navigation Properties
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}
