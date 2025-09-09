using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ec_project_api.Models
{
    public class Ship
    {
        [Key]
        public byte ShipId { get; set; }

        [Required]
        [StringLength(100)]
        public required string CorpName { get; set; }

        public string? Description { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal BaseCost { get; set; }

        [Required]
        [Range(0, 255)]
        public byte EstimatedDays { get; set; }

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