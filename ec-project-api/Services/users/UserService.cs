using ec_project_api.Dtos.response.pagination;
using ec_project_api.Interfaces.Users;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;
using Microsoft.EntityFrameworkCore;

namespace ec_project_api.Services
{
    public class UserService : BaseService<User, int>, IUserService
    {
        public UserService(IUserRepository repository) : base(repository)
        {
        }

        public override async Task<PagedResult<User>> GetAllPagedAsync(QueryOptions<User>? options = null)
        {
            options ??= new QueryOptions<User>();

            options.Includes.Add(u => u.Status);
            options.Includes.Add(u => u.Addresses);
            options.IncludeThen.Add(q => q
                .Include(u => u.UserRoleDetails)
                    .ThenInclude(urd => urd.Role));

            return await base.GetAllPagedAsync(options);
        }

        public override async Task<User?> GetByIdAsync(int id, QueryOptions<User>? options = null)
        {
            options ??= new QueryOptions<User>();

            options.Includes.Add(u => u.Status);
            options.Includes.Add(u => u.Addresses);
            options.Includes.Add(u => u.UserRoleDetails);

            options.IncludeThen.Add(q => q
                .Include(u => u.UserRoleDetails)
                    .ThenInclude(urd => urd.Role));

            return await _repository.GetByIdAsync(id, options);
        }
    }
}
