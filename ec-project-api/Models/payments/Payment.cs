using ec_project_api.Dtos.response.payments;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ec_project_api.Models
{
    public class Payment
    {
        [Key]
        [Column("payment_id")]
        public int PaymentId { get; set; }

        [Column("destination_id")]
        public int? DestinationId { get; set; }                 

        [Column("status_id")]
        public short StatusId { get; set; }

        [Required]
        [Column("amount", TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [StringLength(100)]
        [Column("transaction_id")]
        public string? TransactionId { get; set; }

        [Column("paid_at")]
        public DateTime? PaidAt { get; set; }

        // --- THÊM CÁC TRƯỜNG MỚI CHO SEPAY ---
        public string? QrCodeUrl { get; set; }
        public string? SepayTransactionId { get; set; } // Có thể dùng trường này thay cho TransactionId ở trên, hoặc dùng cả hai
        public string? SepayResponse { get; set; } // Lưu trữ JSON thô từ webhook
        public string? Description { get; set; } // Mô tả giao dịch
        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("DestinationId")]
        public virtual PaymentDestination? PaymentDestination { get; set; }

        [ForeignKey("StatusId")]
        public virtual Status Status { get; set; } = null!;

        public virtual Order? Order { get; set; }

        public static implicit operator Payment(PaymentResponseDto v)
        {
            throw new NotImplementedException();
        }
    }
}