using System.ComponentModel.DataAnnotations;

namespace ec_project_api.Dtos.request.products {
    public class ProductUpdateImageDisplayOrderRequest {
        [Required(ErrorMessage = "Thiếu thông tin id của hình ảnh")]
        public int ProductImageId { get; set; }

        [Required(ErrorMessage = "Thiếu thông tin thứ tự hiển thị của hình ảnh")]
        public byte DisplayOrder { get; set; }
    }
}
