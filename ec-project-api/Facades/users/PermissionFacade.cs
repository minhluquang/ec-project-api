using AutoMapper;
using ec_project_api.Dtos.response.system;
using ec_project_api.Dtos.response.users;
using ec_project_api.Services;

namespace ec_project_api.Facades
{
    public class PermissionFacade
    {
        private readonly IPermissionService _permissionService;
        private readonly IMapper _mapper;

        public PermissionFacade(IPermissionService permissionService, IMapper mapper)
        {
            _permissionService = permissionService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ResourceDto>> GetAllGroupedByResourceAsync()
        {
            var permissions = await _permissionService.GetAllAsync();

            var grouped = permissions
                .GroupBy(p => p.Resource)
                .Select(g =>
                {
                    var resourceDto = _mapper.Map<ResourceDto>(g.Key);
                    resourceDto.Permissions = _mapper.Map<IEnumerable<PermissionDto>>(g);
                    return resourceDto;
                });

            return grouped;
        }
    }
}
