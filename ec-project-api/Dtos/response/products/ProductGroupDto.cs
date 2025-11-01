using ec_project_api.Dtos.Statuses;
using ec_project_api.Models;

namespace ec_project_api.Dtos.response.products {
    public class ProductGroupDto {
        public int ProductGroupId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
    
    public class ProductGroupStatDto : ProductGroupDto {
        public int ProductCount { get; set; }
    }

    public class ProductGroupDetailDto : ProductGroupDto
    {
        public StatusDto? Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}