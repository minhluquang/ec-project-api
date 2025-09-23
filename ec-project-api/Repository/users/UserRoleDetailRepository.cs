using ec_project_api.Interfaces.Users;
using ec_project_api.Models;

public class UserRoleDetailRepository : Repository<UserRoleDetail, int>, IUserRoleDetailRepository
{
    public UserRoleDetailRepository(DataContext context) : base(context) { }
}
