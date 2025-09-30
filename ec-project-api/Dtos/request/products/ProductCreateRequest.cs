using System.ComponentModel.DataAnnotations;

namespace ec_project_api.Dtos.request.products {
    public class ProductCreateRequest {
        [Required(ErrorMessage = "Vui lòng nhâp tên sản phẩm")]
        [StringLength(255, ErrorMessage = "Tên sản phẩm không được vượt quá 255 ký tự")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập slug")]
        [StringLength(255, ErrorMessage = "Slug không được vượt quá 255 ký tự")]
        public string Slug { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng chọn chất liệu")]
        public short MaterialId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn thể loại")]
        public short CategoryId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá cơ bản")]
        [Range(0, 999999999.99, ErrorMessage = "Giá cơ bản phải từ 0 đến 999,999,999.99 VNĐ")]
        public decimal BasePrice { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập phần trăm giảm giá")]
        [Range(0, 100, ErrorMessage = "Phần trăm giảm giá phải từ 0 đến 100")]
        public decimal DiscountPercentage { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn hình ảnh sản phẩm")]
        public IFormFile FileImage { get; set; }
        public string? AltText { get; set; }
    }
}
