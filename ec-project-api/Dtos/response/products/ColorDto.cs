using ec_project_api.Dtos.Statuses;

namespace ec_project_api.Dtos.response.products
{
    public class ColorDto
    {
        public short ColorId { get; set; }
        public string Name { get; set; } = null!;
        
    }
    
    public class ColorStatDto: ColorDto
    {
        public int ProductCount { get; set; }
    }

    public class ColorDetailDto : ColorDto
    {
        public string DisplayName { get; set; } = null!;
        public string HexCode { get; set; } = null!;
        public StatusDto? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
