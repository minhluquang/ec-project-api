using System.ComponentModel.DataAnnotations;

namespace ec_project_api.Dtos.request.suppliers
{
	public class SupplierCreateRequest
	{
		[Required(ErrorMessage = "Vui lòng nhập tên nhà cung cấp")]
		[StringLength(100, ErrorMessage = "Tên nhà cung cấp không được vượt quá 100 ký tự")]
		public string Name { get; set; } = null!;

		[Required(ErrorMessage = "Vui lòng nhập tên người liên hệ")]
		[StringLength(100, ErrorMessage = "Tên người liên hệ không được vượt quá 100 ký tự")]
		public string ContactName { get; set; } = null!;

		[Required(ErrorMessage = "Vui lòng nhập email")]
		[StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
		[EmailAddress(ErrorMessage = "Email không hợp lệ")]
		public string Email { get; set; } = null!;

		[Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
		[StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự")]
		public string Phone { get; set; } = null!;

		[Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
		[StringLength(255, ErrorMessage = "Địa chỉ không được vượt quá 255 ký tự")]
		public string Address { get; set; } = null!;
	}
}
