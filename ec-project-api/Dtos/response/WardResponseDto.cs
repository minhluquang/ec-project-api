namespace ec_project_api.Dtos.response
{
    public class WardResponseDto
    {
        public int Id { get; set; }
        public int ProvinceId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}

