using System.ComponentModel.DataAnnotations;
namespace ec_project_api.Dtos.request.users;

public class UserAvatarImageRequest
{
    [Required(ErrorMessage = "Vui lòng chọn hình ảnh đại diện.")]
    public IFormFile FileImage { get; set; } = null!;
}