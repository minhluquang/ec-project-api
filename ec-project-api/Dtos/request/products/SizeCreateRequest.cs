using System.ComponentModel.DataAnnotations;

namespace ec_project_api.Dtos.request.products
{
    public class SizeCreateRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập tên size")]
        [StringLength(50, ErrorMessage = "Tên size không được vượt quá 50 ký tự")]
        public string Name { get; set; } = null!;
    }
}