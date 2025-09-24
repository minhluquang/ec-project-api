using ec_project_api.Models;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services
{
    public interface IRoleService : IBaseService<Role, short>
    {
        Task<bool> AssignPermissionsAsync(short roleId, IEnumerable<short> permissionIds);
    }
}