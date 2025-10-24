using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ec_project_api.Models
{
    [Table("Wards")]
    public class Ward
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("province_id")]
        public int ProvinceId { get; set; }

        [Required]
        [StringLength(10)]
        [Column("code")]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [ForeignKey("ProvinceId")]
        public virtual Province Province { get; set; } = null!;
        
        public virtual ICollection<Address> Addresses { get; set; } = new HashSet<Address>();
    }
}
