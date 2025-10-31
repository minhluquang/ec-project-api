namespace ec_project_api.Services
{
    public interface IUserRoleService 
    {
        Task<bool> AssignRolesAsync(int userId, IEnumerable<short> roleIds, int? assignedBy = null);
    }
}
