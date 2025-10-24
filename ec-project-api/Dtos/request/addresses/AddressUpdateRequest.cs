using System.ComponentModel.DataAnnotations;
namespace ec_project_api.Dtos.request.addresses;

public class AddressUpdateRequest
{
    [Required(ErrorMessage = "Vui lòng nhập tên người nhận hàng")]
    [StringLength(255, ErrorMessage = "Tên người nhận hàng không được vượt quá 255 ký tự")]
    public string RecipientName { get; set; } = null!;
    
    [RegularExpression(@"^(0|\+84)\d{9}$", ErrorMessage = "Số điện thoại không hợp lệ")]
    [StringLength(12)]
    public string Phone { get; set; } = null!;
    
    [Required(ErrorMessage = "Vui lòng nhập địa chỉ cụ thể")]
    [StringLength(255, ErrorMessage = "Địa chỉ cụ thể không được vượt quá 500 ký tự")]
    public string StreetAddress { get; set; } = null!;
    
    [Required(ErrorMessage = "Vui lòng chọn tỉnh/thành phố")]
    public int ProvinceId { get; set; }
    
    [Required(ErrorMessage = "Vui lòng chọn phường")]
    public int WardId { get; set; }
    
    public bool IsDefault { get; set; } = false;
}