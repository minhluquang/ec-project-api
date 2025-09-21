using ec_project_api.Interfaces.Users;
using ec_project_api.Models;

public class PermissionRepository : Repository<Permission>, IPermissionRepository
{
    public PermissionRepository(DataContext context) : base(context) { }
}
