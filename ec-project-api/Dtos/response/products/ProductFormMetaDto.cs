using ec_project_api.Dtos.Statuses;
using ec_project_api.Models;

namespace ec_project_api.Dtos.response.products {
    public class ProductFormMetaDto {
        public IEnumerable<CategoryDto> Categories { get; set; } = [];
        public IEnumerable<ColorDto> Colors { get; set; } = [];
        public IEnumerable<MaterialDto> Materials {get; set; } = [];
        public IEnumerable<ProductGroupDto> ProductGroups { get; set; } = [];
        public IEnumerable<StatusDto> Statuses { get; set; } = [];
    }
}


