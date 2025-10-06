using System.ComponentModel.DataAnnotations;

public class LoginRequest
{
    [Required(ErrorMessage = "Username là bắt buộc.")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "Password là bắt buộc.")]
    public string Password { get; set; } = null!;
}