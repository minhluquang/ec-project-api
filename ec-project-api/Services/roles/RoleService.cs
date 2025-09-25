using ec_project_api.Constants.Messages;
using ec_project_api.Interfaces.Users;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services
{
    public class RoleService : BaseService<Role, short>, IRoleService
    {
        private readonly IRolePermissionRepository _rolePermissionRepository;

        public RoleService(
            IRoleRepository roleRepository,
            IRolePermissionRepository rolePermissionRepository
        ) : base(roleRepository)
        {
            _rolePermissionRepository = rolePermissionRepository;
        }

        public async Task<bool> AssignPermissionsAsync(short roleId, IEnumerable<short> permissionIds)
        {
            var role = await _repository.GetByIdAsync(roleId);
            if (role == null)
                throw new KeyNotFoundException(RoleMessages.RoleNotFound);

            var oldPermissions = await _rolePermissionRepository.FindAsync(rp => rp.RoleId == roleId);
            var oldIds = oldPermissions.Select(rp => rp.PermissionId).ToHashSet();
            var newIds = permissionIds.ToHashSet();

            var toDelete = oldPermissions.Where(rp => !newIds.Contains(rp.PermissionId));
            foreach (var rp in toDelete)
                await _rolePermissionRepository.DeleteAsync(rp);

            var toAdd = newIds.Except(oldIds);
            foreach (var pid in toAdd)
            {
                var rp = new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = pid,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _rolePermissionRepository.AddAsync(rp);
            }

            await _rolePermissionRepository.SaveChangesAsync();
            return true;
        }

        public override async Task<IEnumerable<Role>> GetAllAsync(QueryOptions<Role>? options = null)
        {
            options ??= new QueryOptions<Role>();
            options.Includes.Add(r => r.Status);
            options.Includes.Add(r => r.RolePermissions);

            return await base.GetAllAsync(options);
        }

        public override async Task<Role?> GetByIdAsync(short id, QueryOptions<Role>? options = null)
        {
            options = new QueryOptions<Role>();
            options.Includes.Add(r => r.Status);
            options.Includes.Add(r => r.RolePermissions);

            var role = await _repository.GetByIdAsync(id, options);

            if (role == null)
                throw new KeyNotFoundException(RoleMessages.RoleNotFound);

            return role;
        }

        public override async Task<bool> DeleteByIdAsync(short id)
        {
            var role = await _repository.GetByIdAsync(id);
            if (role == null)
                throw new KeyNotFoundException(RoleMessages.RoleNotFound);

            await _repository.DeleteAsync(role);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
