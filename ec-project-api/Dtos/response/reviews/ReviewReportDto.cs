using ec_project_api.Dtos.Statuses;
using ec_project_api.Dtos.Users;

namespace ec_project_api.Dtos.response.reviewreports {
    public class ReviewReportDto {
        public int ReviewReportId { get; set; }
        public int ReviewId { get; set; }
        public string Reason { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Basic refs
        public StatusDto? Status { get; set; }
        public UserDto? User { get; set; }
    }
}