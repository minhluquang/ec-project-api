using System.ComponentModel.DataAnnotations;

namespace ec_project_api.Dtos.request.materials
{
    public class MaterialCreateRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập tên chất liệu")]
        [StringLength(100, ErrorMessage = "Tên chất liệu không được vượt quá 100 ký tự")]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }
    }
}