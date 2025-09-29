using ec_project_api.Interfaces.Users;
using ec_project_api.Models;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services
{
    public class UserRoleService 
        : BaseService<UserRoleDetail, int>, IUserRoleService
    {
        public UserRoleService(IUserRoleDetailRepository repository)
            : base(repository)
        {
        }

        public async Task<bool> AssignRolesAsync(int userId, IEnumerable<short> roleIds, int? assignedBy = null)
        {
            var now = DateTime.UtcNow;
            var allUserRoles = await _repository.GetAllAsync();
            var existing = allUserRoles
                .Where(urd => urd.UserId == userId)
                .Select(urd => urd.RoleId)
                .ToList();

            var newRoles = roleIds.Except(existing);

            foreach (var roleId in newRoles)
            {
                await _repository.AddAsync(new UserRoleDetail
                {
                    UserId = userId,
                    RoleId = roleId,
                    AssignedBy = assignedBy,
                    AssignedAt = now
                });
            }

            return await _repository.SaveChangesAsync() > 0;
        }
    }
}
