using System.ComponentModel.DataAnnotations;

namespace ec_project_api.Dtos.request.reviews {
    public class ReviewUpdateRequest {
        [Required(ErrorMessage = "Đánh giá là bắt buộc.")]
        [Range(1, 5, ErrorMessage = "Đánh giá phải từ 1 đến 5 sao.")]
        public byte Rating { get; set; }

        [StringLength(1000, ErrorMessage = "Bình luận không được vượt quá 1000 ký tự.")]
        public string? Comment { get; set; }
        
        public List<IFormFile>? Images { get; set; }
        public List<int>? KeepImageIds { get; set; }
    }
}