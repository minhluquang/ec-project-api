using System.ComponentModel.DataAnnotations;

namespace ec_project_api.Dtos.request.products {
    public class ProductVariantCreateRequest {
        [Required(ErrorMessage = "Vui lòng chọn size")]
        [Range(1, byte.MaxValue, ErrorMessage = "Vui lòng chọn size hợp lệ")]
        public byte SizeId { get; set; }
    }
}
