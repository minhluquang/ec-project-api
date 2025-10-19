using System.ComponentModel.DataAnnotations;

namespace ec_project_api.Dtos.request.payments
{
    public class PaymentMethodUpdateRequest
    {
        [Required(ErrorMessage = "Tên phương thức thanh toán là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên phương thức thanh toán không được vượt quá 100 ký tự.")]
        public string MethodName { get; set; } = null!;

        public short StatusId { get; set; } // Cho phép cập nhật trạng thái
    }
}
