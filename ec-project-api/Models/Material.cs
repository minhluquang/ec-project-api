namespace ec_project_api.Models
{
    public class Material
    {
        public int MaterialId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation collections
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
