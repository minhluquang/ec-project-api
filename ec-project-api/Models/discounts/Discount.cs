using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
﻿namespace ec_project_api.Models
{
    public class Discount
    {
        [Key]
        [Column("discount_id")]
        public int DiscountId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("code")]
        public required string Code { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Required]
        [StringLength(20)]
        [Column("discount_type")]
        public required string DiscountType { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        [Column("discount_value", TypeName = "decimal(18,2)")]
        public decimal DiscountValue { get; set; }

        [Range(0, double.MaxValue)]
        [Column("min_order_amount", TypeName = "decimal(18,2)")]
        public decimal MinOrderAmount { get; set; } = 0.00m;

        [Range(0, double.MaxValue)]
        [Column("max_discount_amount", TypeName = "decimal(18,2)")]
        public decimal? MaxDiscountAmount { get; set; }

        [Column("usage_limit")]
        public int? UsageLimit { get; set; }

        [Range(0, int.MaxValue)]
        [Column("used_count")]
        public int UsedCount { get; set; } = 0;

        [Column("start_at")]
        public DateTime? StartAt { get; set; }

        [Column("end_at")]
        public DateTime? EndAt { get; set; }

        [Column("status_id")]
        public short StatusId { get; set; }

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(StatusId))]
        public virtual Status Status { get; set; } = null!;

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}