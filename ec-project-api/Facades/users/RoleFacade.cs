using AutoMapper;
using ec_project_api.Dtos.response.users;
using ec_project_api.Models;
using ec_project_api.Services;
using ec_project_api.Repository.Base;
using ec_project_api.Constants.variables;
using ec_project_api.Constants.Messages;
using ec_project_api.Dtos.request.users;

namespace ec_project_api.Facades
{
    public class RoleFacade
    {
        private readonly IRoleService _roleService;
        private readonly IStatusService _statusService;
        private readonly IPermissionService _permissionService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public RoleFacade(
            IRoleService roleService,
            IMapper mapper,
            IStatusService statusService,
            IPermissionService permissionService,
            IUserService userService)
        {
            _roleService = roleService;
            _mapper = mapper;
            _statusService = statusService;
            _permissionService = permissionService;
            _userService = userService;
        }

        public async Task<IEnumerable<RoleDto>> GetAllAsync(string? statusName = null)
        {
            var options = BuildRoleQueryOptions(statusName);

            var roles = await _roleService.GetAllAsync(options);

            if (!string.IsNullOrEmpty(statusName) && !roles.Any())
                throw new KeyNotFoundException(RoleMessages.RoleNotFound);

            return _mapper.Map<IEnumerable<RoleDto>>(roles);
        }

        public async Task<RoleDto> GetByIdAsync(short id)
        {
            var role = await _roleService.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(RoleMessages.RoleNotFound);

            return _mapper.Map<RoleDto>(role);
        }

        public async Task<bool> CreateAsync(RoleRequest request)
        {
            await EnsureRoleNameUniqueAsync(request.Name);

            var role = _mapper.Map<Role>(request);

            await EnsureStatusValidAsync(role.StatusId);

            return await _roleService.CreateAsync(role);
        }

        public async Task<bool> UpdateAsync(short id, RoleRequest request)
        {
            var role = await _roleService.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(RoleMessages.RoleNotFound);

            await EnsureRoleNameUniqueAsync(request.Name, id);

            _mapper.Map(request, role);

            await EnsureStatusValidAsync(role.StatusId);

            return await _roleService.UpdateAsync(role);
        }

        public async Task<bool> DeleteByIdAsync(short id)
        {
            var role = await _roleService.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(RoleMessages.RoleNotFound);

            var isAssigned = await IsRoleAssignedToUsers(id);

            if (isAssigned)
                throw new InvalidOperationException(RoleMessages.UserAssignedRole);

            return await _roleService.DeleteByIdAsync(id);
        }

        public async Task AssignPermissionsAsync(short roleId, IEnumerable<short> permissionIds)
        {
            var role = await _roleService.GetByIdAsync(roleId)
                ?? throw new KeyNotFoundException(RoleMessages.RoleNotFound);

            var permissions = await _permissionService.FindAsync(p => permissionIds.Contains(p.PermissionId));

            var missing = permissionIds.Except(permissions.Select(p => p.PermissionId)).ToList();
            if (missing.Any())
            {
                var missingStr = string.Join(", ", missing);
                throw new KeyNotFoundException(string.Format(PermissionMessages.PermissionsNotFound, missingStr));
            }

            var success = await _roleService.AssignPermissionsAsync(roleId, permissionIds);
            if (!success)
                throw new KeyNotFoundException(RoleMessages.RoleNotFound);
        }

        // ==============================
        // Helpers
        // ==============================

        private async Task EnsureRoleNameUniqueAsync(string roleName, short? excludeId = null)
        {
            var existing = await _roleService.FirstOrDefaultAsync(
                r => r.Name == roleName && (!excludeId.HasValue || r.RoleId != excludeId.Value)
            );

            if (existing != null)
                throw new InvalidOperationException(RoleMessages.RoleAlreadyExists);
        }
        
         private async Task<bool> IsRoleAssignedToUsers(short roleId)
        {
            var users = await _userService.FindAsync(u =>
                u.UserRoleDetails.Any(urd => urd.RoleId == roleId));

            return users.Any();
        }

        private static QueryOptions<Role> BuildRoleQueryOptions(string? statusName)
        {
            var options = new QueryOptions<Role>
            {
                Includes = { r => r.Status, r => r.RolePermissions }
            };

            if (!string.IsNullOrEmpty(statusName))
            {
                options.Filter = r =>
                    r.Status != null &&
                    r.Status.Name == statusName &&
                    r.Status.EntityType == EntityVariables.Role;
            }

            return options;
        }

        private async Task EnsureStatusValidAsync(short statusId)
        {
            var status = await _statusService.GetByIdAsync(statusId, new QueryOptions<Status>
            {
                Filter = s => s.EntityType == EntityVariables.Role
            });

            if (status == null)
                throw new InvalidOperationException(StatusMessages.StatusNotFound);
        }
    }
}
