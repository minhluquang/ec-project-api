using System.ComponentModel.DataAnnotations;

namespace ec_project_api.Dtos.request.reviews {
    public class ReviewReportCreateRequest {
        [Required(ErrorMessage = "Vui lòng chọn lý do báo cáo")]
        public string Reason { get; set; } = null!;

        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
        public string? Description { get; set; }
    }
}