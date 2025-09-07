namespace ec_project_api.Models
{
    public class Review
    {
        public int ReviewId { get; set; }
        public string Content { get; set; }
        public decimal Rating { get; set; }
        public int HelpfulCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation Collections
        public ICollection<ReviewImage> ReviewImages { get; set; } = new List<ReviewImage>();

        // Foreign Keys + Navigation Properties
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}
