using System;
using System.ComponentModel.DataAnnotations;

namespace ec_project_api.Models
{
    public class DiscountCreateRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập mã khuyến mãi.")]
        [StringLength(50)]
        public required string Code { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập kiểu khuyến mãi.")]
        [StringLength(20)]
        public required string DiscountType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập giá trị khuyến mãi.")]
        [Range(0, double.MaxValue)]
        public decimal DiscountValue { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập giá trị đơn tối thiểu.")]
        [Range(0, double.MaxValue)]
        public decimal MinOrderAmount { get; set; } = 0.00m;
        [Required(ErrorMessage = "Vui lòng nhập giá trị giảm tối đa.")]
        [Range(0, double.MaxValue)]
        public decimal? MaxDiscountAmount { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập ngày bắt đầu.")]
        public DateTime? StartAt { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập ngày kết thúc.")]
        public DateTime? EndAt { get; set; }
        public int? UsageLimit { get; set; }


    }
}