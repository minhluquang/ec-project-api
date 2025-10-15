using System.ComponentModel.DataAnnotations;

namespace ec_project_api.Dtos.request.shipping
{
    public class ShipUpdateRequest
    {
        [Required(ErrorMessage = "Id vận chuyển là bắt buộc")]
        public byte ShipId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên doanh nghiệp vận chuyển")]
        [StringLength(100, ErrorMessage = "Tên doanh nghiệp không được vượt quá 100 ký tự")]
        public string CorpName { get; set; } = null!;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập chi phí cơ bản")]
        [Range(0, double.MaxValue, ErrorMessage = "Chi phí phải lớn hơn hoặc bằng 0")]
        public decimal BaseCost { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số ngày ước tính")]
        [Range(0, 255, ErrorMessage = "Số ngày ước tính phải trong khoảng 0-255")]
        public byte EstimatedDays { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn trạng thái")]
        public short StatusId { get; set; }
    }
}
