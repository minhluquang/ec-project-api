using System.ComponentModel.DataAnnotations;

namespace ec_project_api.Dtos.request.users
{
    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "UserId không được để trống.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Mật khẩu cũ không được để trống.")]
        public string OldPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu mới không được để trống.")]
        [StringLength(60, MinimumLength = 8, ErrorMessage = "Mật khẩu mới phải có từ 8 đến 60 ký tự.")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&]).+$", 
            ErrorMessage = "Mật khẩu mới phải chứa chữ cái, số và ký tự đặc biệt.")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống.")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
