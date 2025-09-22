using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ec_project_api.Models
{
    public class Status
    {
        [Key]
        [Column("status_id")]
        public int StatusId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("name")]
        public required string Name { get; set; }

        [Column("display_name")]
        public string? DisplayName { get; set; }

        [Required]
        [StringLength(50)]
        [Column("entity_type")]
        public required string EntityType { get; set; }

        public string UniqueNameEntity => $"{Name}_{EntityType}";

        public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
        public virtual ICollection<Size> Sizes { get; set; } = new List<Size>();
        public virtual ICollection<Supplier> Suppliers { get; set; } = new List<Supplier>();
        public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
        public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
        public virtual ICollection<User> Users { get; set; } = new List<User>();
        public virtual ICollection<Color> Colors { get; set; } = new List<Color>();
        public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
        public virtual ICollection<Material> Materials { get; set; } = new List<Material>();
        public virtual ICollection<Ship> Ships { get; set; } = new List<Ship>();
        public virtual ICollection<Discount> Discounts { get; set; } = new List<Discount>();
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
        public virtual ICollection<PaymentMethod> PaymentMethods { get; set; } = new List<PaymentMethod>();
        public virtual ICollection<PaymentDestination> PaymentDestinations { get; set; } = new List<PaymentDestination>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<ProductReturn> ProductReturns { get; set; } = new List<ProductReturn>();
    }
}