namespace ec_project_api.Dtos.response.products
{
    public class ProductImageDto
    {
        public int ProductImageId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string? AltText { get; set; }
    }

    public class ProductImageDetailDto : ProductImageDto
    {
        public bool IsPrimary { get; set; }
        public short DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
