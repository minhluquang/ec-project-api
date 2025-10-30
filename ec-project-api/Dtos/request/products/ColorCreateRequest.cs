using System.ComponentModel.DataAnnotations;

namespace ec_project_api.Dtos.request.products
{
    public class ColorCreateRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập tên màu sắc")]
        [StringLength(50, ErrorMessage = "Tên màu sắc không được vượt quá 50 ký tự")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập tên hiển thị")]
        [StringLength(50, ErrorMessage = "Tên hiển thị không được vượt quá 50 ký tự")]
        public string? DisplayName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã màu hex")]
        [StringLength(7, ErrorMessage = "Mã màu hex không hợp lệ")]
        public string? HexCode { get; set; }
    }
}