using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ec_project_api.Models
{
    public class ReviewImage
    {
        [Key]
        public int ReviewImageId { get; set; }

        [Required]
        public int ReviewId { get; set; }

        [Required]
        [StringLength(255)]
        public required string ImageUrl { get; set; }

        [StringLength(255)]
        public string? AltText { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("ReviewId")]
        public virtual Review? Review { get; set; }
    }
}