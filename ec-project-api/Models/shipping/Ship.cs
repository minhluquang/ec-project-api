using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ec_project_api.Models
{
    public class Ship
    {
        [Key]
        [Column("ship_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte ShipId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("corp_name")]
        public required string CorpName { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        [Column("base_cost", TypeName = "decimal(18,2)")]
        public decimal BaseCost { get; set; }

        [Required]
        [Range(0, 255)]
        [Column("estimated_days")]
        public byte EstimatedDays { get; set; }

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
