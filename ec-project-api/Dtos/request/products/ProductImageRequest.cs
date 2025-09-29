using System.ComponentModel.DataAnnotations;

namespace ec_project_api.Dtos.request.products {
    public class ProductImageRequest {
        [Required(ErrorMessage = "Vui lòng chọn hình ảnh.")]
        public IFormFile FileImage { get; set; } = null!;
        public string? AltText { get; set; }
        public bool IsPrimary { get; set; } = false;
    }
}
