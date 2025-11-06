using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ec_project_api.Models
{
    public class Review
    {
        [Key]
        [Column("review_id")]
        public int ReviewId { get; set; }

        [Required]
        [Column("order_item_id")]
        public int OrderItemId { get; set; }

        [Required]
        [Range(1, 5)]
        [Column("rating")]
        public short Rating { get; set; }

        [Column("comment", TypeName = "nvarchar(max)")]
        public string? Comment { get; set; }

        [Column("is_edited")]
        public bool IsEdited { get; set; } = false;  

        [Column("status_id")]
        public short StatusId { get; set; }

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("OrderItemId")]
        public virtual OrderItem? OrderItem { get; set; }

        [ForeignKey("StatusId")]
        public virtual Status Status { get; set; } = null!;
        public virtual ICollection<ReviewImage> ReviewImages { get; set; } = new List<ReviewImage>();
    }
}