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

            var existingRoles = (await _repository.GetAllAsync())
                .Where(urd => urd.UserId == userId)
                .ToList();

            foreach (var userRole in existingRoles)
            {
                await _repository.DeleteAsync(userRole);
            }

            foreach (var roleId in roleIds.Distinct())
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
