using ec_project_api.Dtos.response.orders;
using ec_project_api.Dtos.Statuses;
using ec_project_api.Models;

namespace ec_project_api.Dtos.response.reviews {
    public class ReviewDto {
        public int ReviewId { get; set; }
        public byte Rating { get; set; }
        public string? Comment { get; set; }
        public bool IsEdited { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Basic refs
        public StatusDto? Status { get; set; }
        public OrderItemDto? OrderItem { get; set; }
        public List<ReviewImageDto> ReviewImages { get; set; } = new List<ReviewImageDto>();
        public string? Username { get; set; }
    }
}