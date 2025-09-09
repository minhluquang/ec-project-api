using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ec_project_api.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int OrderItemId { get; set; }

        [Required]
        [Range(1, 5)]
        public byte Rating { get; set; }

        public string? Comment { get; set; }

        public int? StatusId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }

        [ForeignKey("OrderItemId")]
        public virtual OrderItem? OrderItem { get; set; }

        [ForeignKey("StatusId")]
        public virtual Status? Status { get; set; }

        public virtual ICollection<ReviewImage> ReviewImages { get; set; } = new List<ReviewImage>();
    }
}