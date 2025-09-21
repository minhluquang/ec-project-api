using ec_project_api.Interfaces.Users;
using ec_project_api.Models;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(DataContext context) : base(context) { }
}
