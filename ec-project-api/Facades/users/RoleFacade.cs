using AutoMapper;
using ec_project_api.Dtos.response.users;
using ec_project_api.Models;
using ec_project_api.Services;

namespace ec_project_api.Facades
{
    public class RoleFacade
    {
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public RoleFacade(IRoleService roleService, IMapper mapper)
        {
            _roleService = roleService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RoleDto>> GetAllAsync()
        {
            var roles = await _roleService.GetAllAsync();
            return _mapper.Map<IEnumerable<RoleDto>>(roles);
        }

        public async Task<RoleDto?> GetByIdAsync(short id)
        {
            var role = await _roleService.GetByIdAsync(id);
            return role == null ? null : _mapper.Map<RoleDto>(role);
        }

        public async Task<RoleDto> CreateAsync(RoleRequest request)
        {
            var role = _mapper.Map<Role>(request);
            var created = await _roleService.CreateAsync(role);
            return _mapper.Map<RoleDto>(created);
        }

        public async Task<bool> UpdateAsync(short id, RoleRequest request)
        {
            var existingRole = await _roleService.GetByIdAsync(id);
            if (existingRole == null)
                return false;

            _mapper.Map(request, existingRole);
            return await _roleService.UpdateAsync(existingRole);
        }

        public async Task<bool> DeleteByIdAsync(short id)
        {
            return await _roleService.DeleteByIdAsync(id);
        }

        public async Task<bool> AssignPermissionsAsync(short roleId, IEnumerable<short> permissionIds)
        {
            return await _roleService.AssignPermissionsAsync(roleId, permissionIds);
        }
    }
}
