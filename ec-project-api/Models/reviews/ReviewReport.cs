using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ec_project_api.Models {
    [Table("ReviewReports")]
    public class ReviewReport {
        [Key]
        [Column("review_report_id")]
        public int ReviewReportId { get; set; }

        [Required]
        [Column("review_id")]
        public int ReviewId { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("status_id")]
        public short StatusId { get; set; }

        [Required]
        [StringLength(255)]
        [Column("reason")]
        public string Reason { get; set; } = null!;

        [Column("description")]
        public string? Description { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public virtual Review Review { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual Status Status { get; set; } = null!;
    }
}