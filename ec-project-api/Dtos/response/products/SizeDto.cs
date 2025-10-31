using ec_project_api.Dtos.Statuses;

namespace ec_project_api.Dtos.response.products
{
    public class SizeDto
    {   
        public byte SizeId { get; set; }
        public string Name { get; set; } = null!;
    }

    public class SizeDetailDto : SizeDto
    {
        public StatusDto? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
