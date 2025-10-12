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
            var options = new QueryOptions<Role>
            {
                Includes = { r => r.Status, r => r.RolePermissions }
            };

            if (!string.IsNullOrEmpty(statusName))
            {
                options.Filter = r => r.Status != null
                                   && r.Status.Name == statusName
                                   && r.Status.EntityType == EntityVariables.Role;
            }

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
            if (await _roleService.FirstOrDefaultAsync(r => r.Name == request.Name) != null)
                throw new InvalidOperationException(RoleMessages.RoleAlreadyExists);

            var role = _mapper.Map<Role>(request);

            var status = await _statusService.GetByIdAsync(role.StatusId, new QueryOptions<Status>
            {
                Filter = s => s.EntityType == EntityVariables.Role
            });

            if (status == null)
                throw new InvalidOperationException(StatusMessages.StatusNotFound);

            return await _roleService.CreateAsync(role);
        }

        public async Task<bool> UpdateAsync(short id, RoleRequest request)
        {
            var role = await _roleService.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(RoleMessages.RoleNotFound);

            if (await _roleService.FirstOrDefaultAsync(r => r.Name == request.Name && r.RoleId != id) != null)
                throw new InvalidOperationException(RoleMessages.RoleAlreadyExists);

            _mapper.Map(request, role);

            var status = await _statusService.GetByIdAsync(role.StatusId, new QueryOptions<Status>
            {
                Filter = s => s.EntityType == EntityVariables.Role
            });

            if (status == null)
                throw new InvalidOperationException(StatusMessages.StatusNotFound);

            return await _roleService.UpdateAsync(role);
        }

        public async Task<bool> DeleteByIdAsync(short id)
        {
            var role = await _roleService.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(RoleMessages.RoleNotFound);

            var hasUsers = (await _userService.FindAsync(u =>
                u.UserRoleDetails.Any(urd => urd.RoleId == id))).Any();

            if (hasUsers)
                throw new InvalidOperationException(RoleMessages.UserAssignedRole);

            return await _roleService.DeleteByIdAsync(id);
        }

        public async Task AssignPermissionsAsync(short roleId, IEnumerable<short> permissionIds)
        {
            var role = await _roleService.GetByIdAsync(roleId)
                ?? throw new KeyNotFoundException(RoleMessages.RoleNotFound);

            var permissions = await _permissionService.FindAsync(p => permissionIds.Contains(p.PermissionId));
            var foundIds = permissions.Select(p => p.PermissionId).ToHashSet();
            var notFound = permissionIds.Except(foundIds).ToList();

            if (notFound.Any())
                throw new KeyNotFoundException(
                    string.Format(PermissionMessages.PermissionsNotFound, string.Join(", ", notFound))
                );

            if (!await _roleService.AssignPermissionsAsync(roleId, permissionIds))
                throw new KeyNotFoundException(RoleMessages.RoleNotFound);
        }
    }
}
