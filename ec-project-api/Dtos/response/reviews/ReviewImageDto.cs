namespace ec_project_api.Dtos.response.reviews {
    public class ReviewImageDto {
        public int ReviewImageId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string? AltText { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}