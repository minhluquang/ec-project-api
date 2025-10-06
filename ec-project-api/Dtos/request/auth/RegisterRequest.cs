using System.ComponentModel.DataAnnotations;

public class RegisterRequest
{
    [Required(ErrorMessage = "Username là bắt buộc.")]
    [StringLength(50, ErrorMessage = "Username không được vượt quá 50 ký tự.")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "Email là bắt buộc.")]
    [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
    [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự.")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password là bắt buộc.")]
    [StringLength(60, MinimumLength = 6, ErrorMessage = "Password phải có ít nhất 6 ký tự và tối đa 60 ký tự.")]
    public string Password { get; set; } = null!;
}