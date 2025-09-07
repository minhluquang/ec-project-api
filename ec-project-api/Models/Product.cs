namespace ec_project_api.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public decimal BasePrice{ get; set; }
        public decimal SalePrice { get; set; }
        public int DiscountPercentage { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation Collections
        public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();


        // Foreign Keys + Navigation Properties
        public int CategoryId { get; set; } 
        public Category Category { get; set; } = null!;

        public int MaterialId { get; set; }
        public Material Material { get; set; } = null!;
    }
}
