using ec_project_api.Dtos.Statuses;
using ec_project_api.Models;

namespace ec_project_api.Dtos.response.products {
    public class ProductGroupDto {
        public int ProductGroupId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        // keep status as primitive fields to avoid back-references/cycles
        public short StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
    }

    public class ProductGroupDetailDto : ProductGroupDto
    {
        // include Status as a flat DTO (StatusDto must NOT contain navigation collections)
        public StatusDto? Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}