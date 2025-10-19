using System.ComponentModel.DataAnnotations;

namespace ec_project_api.Dtos.request.users
{
    public class RoleRequest
    {
        [Required(ErrorMessage = "Tên vai trò là bắt buộc.")]
        [StringLength(50, ErrorMessage = "Tên vai trò không được vượt quá 50 ký tự.")]
        public string Name { get; set; } = null!;

        [StringLength(255, ErrorMessage = "Mô tả không được vượt quá 255 ký tự.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
        public short? StatusId { get; set; }
    }
}