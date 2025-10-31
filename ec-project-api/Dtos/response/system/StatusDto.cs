namespace ec_project_api.Dtos.Statuses
{
    public class StatusDto
    {
        public short StatusId { get; set; }
        public string Name { get; set; } = null!;
        public string? DisplayName { get; set; }
        public string EntityType { get; set; } = null!;
    }
}
