using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ec_project_api.Models
{
    public class PurchaseOrder
    {
        [Key]
        public int PurchaseOrderId { get; set; }

        [Required]
        public int SupplierId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required]
        public int StatusId { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("SupplierId")]
        public virtual Supplier? Supplier { get; set; }

        [ForeignKey("StatusId")]
        public virtual Status? Status { get; set; }

        public virtual ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();
    }
}