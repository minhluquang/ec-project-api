using System.ComponentModel.DataAnnotations;
using ec_project_api.Constants.variables;

namespace ec_project_api.Dtos.request.users
{
    public class UserRequest
    {
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc.")]
        [StringLength(50, ErrorMessage = "Tên đăng nhập không được vượt quá 50 ký tự.")]
        [RegularExpression(GlobalVariables.UsernameRegex, ErrorMessage = "Tên đăng nhập không hợp lệ.")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Email là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự.")]
        [RegularExpression(GlobalVariables.EmailRegex, ErrorMessage = "Định dạng email không hợp lệ.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [RegularExpression(GlobalVariables.PasswordStrongRegex,
            ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự, bao gồm chữ hoa, chữ thường, số và ký tự đặc biệt.")]
        public string PasswordHash { get; set; } = null!; 

        [StringLength(255, ErrorMessage = "Đường dẫn ảnh không được vượt quá 255 ký tự.")]
        [RegularExpression(GlobalVariables.ImageUrlRegex, ErrorMessage = "Đường dẫn ảnh không hợp lệ.")]
        public string? ImageUrl { get; set; }

        [StringLength(100, ErrorMessage = "Họ và tên không được vượt quá 100 ký tự.")]
        [RegularExpression(GlobalVariables.FullNameRegex, ErrorMessage = "Họ và tên chứa ký tự không hợp lệ.")]
        public string? FullName { get; set; }

        [StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 số.")]
        [RegularExpression(GlobalVariables.PhoneRegex, ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string? Phone { get; set; }

        [StringLength(10, ErrorMessage = "Giới tính không được vượt quá 10 ký tự.")]
        [RegularExpression(GlobalVariables.GenderRegex, ErrorMessage = "Giá trị giới tính không hợp lệ.")]
        public string? Gender { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Ngày sinh phải là kiểu ngày.")]
        public DateTime? DateOfBirth { get; set; }

        public bool IsVerified { get; set; } = false;

        [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
        public int StatusId { get; set; }

        public ICollection<short> RoleIds { get; set; } = new List<short>();
    }
}
