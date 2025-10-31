using ec_project_api.Interfaces.Users;
using ec_project_api.Models;

public class RoleRepository : Repository<Role, short>, IRoleRepository
{
    public RoleRepository(DataContext context) : base(context) { }
}
