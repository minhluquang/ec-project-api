using System.ComponentModel.DataAnnotations;

namespace ec_project_api.Dtos.request.products {
    public class ProductUpdateRequest {
        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        [StringLength(255, ErrorMessage = "Tên sản phẩm không được vượt quá 255 ký tự")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập slug")]
        [StringLength(255, ErrorMessage = "Slug không được vượt quá 255 ký tự")]
        public string Slug { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng chọn màu sắc")]
        [Range(1, short.MaxValue, ErrorMessage = "Vui lòng chọn màu sắc hợp lệ")]
        public short ColorId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn chất liệu")]
        public short MaterialId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn thể loại")]
        public short CategoryId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập phần trăm giảm giá")]
        [Range(0, 100, ErrorMessage = "Phần trăm giảm giá phải từ 0 đến 100")]
        public decimal DiscountPercentage { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn trạng thái")]
        public short StatusId { get; set; }
    }
}
