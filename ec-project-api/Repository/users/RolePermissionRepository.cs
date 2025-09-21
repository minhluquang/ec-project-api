using ec_project_api.Interfaces.Users;
using ec_project_api.Models;

public class RolePermissionRepository : Repository<RolePermission>, IRolePermissionRepository
{
    public RolePermissionRepository(DataContext context) : base(context) { }
}
