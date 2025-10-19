using System.ComponentModel.DataAnnotations;

namespace ec_project_api.Dtos.request.products {
    public class ProductCreateRequest {
        [Required(ErrorMessage = "Vui lòng nhâp tên sản phẩm")]
        [StringLength(255, ErrorMessage = "Tên sản phẩm không được vượt quá 255 ký tự")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập slug")]
        [StringLength(255, ErrorMessage = "Slug không được vượt quá 255 ký tự")]
        public string Slug { get; set; } = null!;


        [Required(ErrorMessage = "Vui lòng chọn màu sắc")]
        [Range(1, short.MaxValue, ErrorMessage = "Vui lòng chọn màu sắc hợp lệ")]
        public short ColorId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn chất liệu")]
        [Range(1, short.MaxValue, ErrorMessage = "Vui lòng chọn màu sắc hợp lệ")]
        public short MaterialId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn thể loại")]
        [Range(1, short.MaxValue, ErrorMessage = "Vui lòng chọn màu sắc hợp lệ")]
        public short CategoryId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn nhóm sản phẩm")]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn nhóm sản phẩm hợp lệ")]
        public int ProductGroupId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn hình ảnh sản phẩm")]
        public IFormFile FileImage { get; set; } = null!;
        public string? AltText { get; set; }
    }
}
