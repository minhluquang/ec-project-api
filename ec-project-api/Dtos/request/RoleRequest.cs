namespace ec_project_api.Dtos.response.users
{
    public class RoleRequest
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int StatusId { get; set; }
    }
} 