namespace ec_project_api.Models
{
    public class ReviewImage
    {
        public int ReviewImageId { get; set; }
        public string ImageUrl { get; set; }
        public string? AltText { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Foreign Keys + Navigation Properties
        public int ReviewId { get; set; }
        public Review Review { get; set; } = null!;
    }
}
