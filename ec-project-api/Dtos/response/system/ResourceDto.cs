using ec_project_api.Dtos.response.users;

namespace ec_project_api.Dtos.response.system
{
    public class ResourceDto
    {
        public short ResourceId { get; set; }
        public string ResourceName { get; set; } = string.Empty;
        public string? ResourceDescription { get; set; }

        public IEnumerable<PermissionDto> Permissions { get; set; } = new List<PermissionDto>();
    }
}
