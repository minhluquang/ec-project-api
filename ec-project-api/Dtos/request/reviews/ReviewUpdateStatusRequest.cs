using System.ComponentModel.DataAnnotations;

namespace ec_project_api.Dtos.request.reviews {
    public class ReviewUpdateStatusRequest {
        [Required(ErrorMessage = "Vui lòng chọn trạng thái")]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn trạng thái hợp lệ")]
        public short StatusId { get; set; }
    }
}
