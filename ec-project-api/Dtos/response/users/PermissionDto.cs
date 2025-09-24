namespace ec_project_api.Dtos.response.users
{
    public class PermissionDto
    {
        public short PermissionId { get; set; }
        public string PermissionName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
