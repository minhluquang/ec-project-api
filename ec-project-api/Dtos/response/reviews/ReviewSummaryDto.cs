namespace ec_project_api.Dtos.response.reviews
{
    public class ReviewSummaryDto
    {
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public Dictionary<int, int> ReviewDetails { get; set; } = new Dictionary<int, int>();
        public int HasImageCount { get; set; }
    }
}