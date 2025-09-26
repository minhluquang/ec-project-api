using AutoMapper;
using ec_project_api.Dtos.response.users;
using ec_project_api.Models;
using ec_project_api.Services;
using ec_project_api.Repository.Base;
using ec_project_api.Constants.variables;
using ec_project_api.Constants.Messages;

namespace ec_project_api.Facades
{
    public class RoleFacade
    {
        private readonly IRoleService _roleService;
        private readonly IStatusService _statusService;
        private readonly IPermissionService _permissionService;
        private readonly IMapper _mapper;

        public RoleFacade(IRoleService roleService, IMapper mapper, IStatusService statusService, IPermissionService permissionService)
        {
            _roleService = roleService;
            _mapper = mapper;
            _statusService = statusService;
            _permissionService = permissionService;
        }

        public async Task<IEnumerable<RoleDto>> GetAllAsync(string? statusName = null)
        {
            var options = new QueryOptions<Role>
            {
                Includes = { r => r.Status, r => r.RolePermissions }
            };

            if (!string.IsNullOrEmpty(statusName))
            {
                options.Filter = r => r.Status != null && r.Status.Name == statusName && r.Status.EntityType == EntityVariables.Role;
            }

            var roles = await _roleService.GetAllAsync(options);
            return _mapper.Map<IEnumerable<RoleDto>>(roles);
        }

        public async Task<RoleDto> GetByIdAsync(short id)
        {
            var role = await _roleService.GetByIdAsync(id);
            return _mapper.Map<RoleDto>(role);
        }

        public async Task<bool> CreateAsync(RoleRequest request)
        {
            var existingRole = await _roleService.FirstOrDefaultAsync(
                r => r.Name == request.Name
            );

            if (existingRole != null)
                throw new InvalidOperationException(RoleMessages.RoleAlreadyExists);

            var role = _mapper.Map<Role>(request);

            await _statusService.GetByIdAsync(role.StatusId, new QueryOptions<Status>
            {
                Filter = s => s.EntityType == EntityVariables.Role
            });

            return await _roleService.CreateAsync(role);
        }

        public async Task<bool> UpdateAsync(short id, RoleRequest request)
        {
            var existingRole = await _roleService.GetByIdAsync(id);

            if (existingRole == null)
                throw new KeyNotFoundException(RoleMessages.RoleNotFound);

            var duplicateRole = await _roleService.FirstOrDefaultAsync(
                r => r.Name == request.Name && r.RoleId != id
            );

            if (duplicateRole != null)
                throw new InvalidOperationException(RoleMessages.RoleAlreadyExists);

            _mapper.Map(request, existingRole);

            await _statusService.GetByIdAsync(existingRole.StatusId, new QueryOptions<Status>
            {
                Filter = s => s.EntityType == EntityVariables.Role
            });

            return await _roleService.UpdateAsync(existingRole);
        }

        public async Task<bool> DeleteByIdAsync(short id)
        {
            return await _roleService.DeleteByIdAsync(id);
        }

        public async Task AssignPermissionsAsync(short roleId, IEnumerable<short> permissionIds)
        {
            var permissions = await _permissionService.FindAsync(p => permissionIds.Contains(p.PermissionId));
            var foundIds = permissions.Select(p => p.PermissionId).ToHashSet();
            var notFound = permissionIds.Except(foundIds).ToList();

            if (notFound.Any())
                throw new KeyNotFoundException(
                    string.Format(PermissionMessages.PermissionsNotFound, string.Join(", ", notFound))
                );

            await _roleService.AssignPermissionsAsync(roleId, permissionIds);
        }
    }
}
