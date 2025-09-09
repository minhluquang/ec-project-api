using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
﻿namespace ec_project_api.Models
{
    public class Discount
    {
        [Key]
        public int DiscountId { get; set; }

        [Required]
        [StringLength(50)]
        public required string Code { get; set; }

        public string? Description { get; set; }

        [Required]
        [StringLength(20)]
        public required string DiscountType { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal DiscountValue { get; set; }

        [Range(0, double.MaxValue)]
        public decimal MinOrderAmount { get; set; } = 0.00m;

        [Range(0, double.MaxValue)]
        public decimal? MaxDiscountAmount { get; set; }

        public int? UsageLimit { get; set; }

        [Range(0, int.MaxValue)]
        public int UsedCount { get; set; } = 0;

        public DateTime? StartAt { get; set; }

        public DateTime? EndAt { get; set; }

        [Required]
        public int StatusId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("StatusId")]
        public virtual Status? Status { get; set; }

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}