using ec_project_api.Dtos.Statuses;

namespace ec_project_api.Dtos.response.products
{
    public class CategoryDto
    {
        public short CategoryId { get; set; }
        public string Name { get; set; } = null!;
    }

    public class CategoryDetailDto : CategoryDto
    {
        public string Slug { get; set; } = null!;
        public string? Description { get; set; }  
        public string? SizeDetail { get; set; }   
        public StatusDto Status { get; set; } = null!;
        public short? ParentId { get; set; }   
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}