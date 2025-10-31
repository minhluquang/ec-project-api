using ec_project_api.Dtos.Statuses;

namespace ec_project_api.Dtos.response.users
{
    public class RoleDto
    {
        public short RoleId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public StatusDto Status { get; set; } = null!;
        public IEnumerable<short> PermissionIds { get; set; } = new List<short>();
    }
} 