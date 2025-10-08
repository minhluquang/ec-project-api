using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ec_project_api.Models {
    public class ReviewImage {
        [Key]
        [Column("review_image_id")]
        public int ReviewImageId { get; set; }

        [Required]
        [Column("review_id")]
        public int ReviewId { get; set; }

        [StringLength(255)]
        [Column("image_url")]
        public string? ImageUrl { get; set; }

        [StringLength(255)]
        [Column("alt_text")]
        public string? AltText { get; set; }

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(ReviewId))]
        public virtual Review? Review { get; set; }
    }
}