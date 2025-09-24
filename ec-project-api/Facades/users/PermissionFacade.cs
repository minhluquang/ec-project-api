using ec_project_api.Dtos.response.system;
using ec_project_api.Dtos.response.users;
using ec_project_api.Services;

namespace ec_project_api.Facades
{
    public class PermissionFacade
    {
        private readonly IPermissionService _permissionService;

        public PermissionFacade(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        public async Task<IEnumerable<ResourceDto>> GetAllGroupedByResourceAsync()
        {
            var permissions = await _permissionService.GetAllAsync();

            var grouped = permissions
                .GroupBy(p => p.Resource)
                .Select(g => new ResourceDto
                {
                    ResourceId = g.Key.ResourceId,
                    ResourceName = g.Key.Name,
                    ResourceDescription = g.Key.Description,
                    Permissions = g.Select(p => new PermissionDto
                    {
                        PermissionId = p.PermissionId,
                        PermissionName = p.PermissionName,
                        Description = p.Description
                    })
                });

            return grouped;
        }
    }
}
