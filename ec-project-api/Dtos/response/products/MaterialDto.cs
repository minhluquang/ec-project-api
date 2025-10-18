using ec_project_api.Dtos.Statuses;

namespace ec_project_api.Dtos.response.products
{
    public class MaterialDto
    {
        public short MaterialId { get; set; }
        public string Name { get; set; } = null!;
    }
    
    public class MaterialStatDto : MaterialDto
    {
        public int ProductCount { get; set; }
    }

    public class MaterialDetailDto : MaterialDto
    {
        public string? Description { get; set; } 
        public StatusDto Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
