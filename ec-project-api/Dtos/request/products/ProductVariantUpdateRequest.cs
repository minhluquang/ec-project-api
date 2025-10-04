using System.ComponentModel.DataAnnotations;

namespace ec_project_api.Dtos.request.products {
    public class ProductVariantUpdateRequest {
        [Required(ErrorMessage = "Vui lòng chọn màu sắc")]
        [Range(1, short.MaxValue, ErrorMessage = "Vui lòng chọn màu sắc hợp lệ")]
        public short ColorId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn size")]
        [Range(1, byte.MaxValue, ErrorMessage = "Vui lòng chọn size hợp lệ")]
        public byte SizeId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn trạng thái")]
        public int StatusId { get; set; }
    }
}
