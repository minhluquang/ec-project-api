namespace ec_project_api.Dtos.request.users
{
    public class AssignRolesRequest
    {
        public int UserId { get; set; }
        public ICollection<short> RoleIds { get; set; } = new List<short>();
    }
}
