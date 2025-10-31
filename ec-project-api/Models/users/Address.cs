using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ec_project_api.Models {
    public class Address
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("address_id")]
        public int AddressId { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("recipient_name")]
        public string RecipientName { get; set; } = string.Empty;

        [Required]
        [StringLength(15)]
        [Column("phone")]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [Column("street_address")]
        public string StreetAddress { get; set; } = string.Empty;

        [Required]
        [Column("province_id")]
        public int ProvinceId { get; set; }
        
        [Required]
        [Column("ward_id")]
        public int WardId { get; set; }

        [Required]
        [Column("is_default")]
        public bool IsDefault { get; set; } = false;

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        [ForeignKey(nameof(UserId))]
        public virtual User? User { get; set; }
        
        [ForeignKey(nameof(ProvinceId))]
        public virtual Province Province { get; set; } = null!;
        
        [ForeignKey(nameof(WardId))]
        public virtual Ward Ward { get; set; } = null!;
    }
}