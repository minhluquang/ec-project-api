using System.ComponentModel.DataAnnotations;

public class ResetPasswordRequest
{
    [Required(ErrorMessage = "Token là bắt buộc.")]
    public string Token { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
    [StringLength(60, MinimumLength = 8, ErrorMessage = "Mật khẩu phải có từ 8 đến 60 ký tự.")]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&]).+$", 
        ErrorMessage = "Mật khẩu phải chứa chữ cái, số và ký tự đặc biệt.")]
    public string Password { get; set; } = null!;
}