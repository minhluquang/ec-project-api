using ec_project_api.Interfaces.Users;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services
{
    public class PermissionService 
        : BaseService<Permission, short>, IPermissionService
    {
        public PermissionService(IPermissionRepository repository)
            : base(repository)
        {
        }

        public override async Task<IEnumerable<Permission>> GetAllAsync(QueryOptions<Permission>? options = null)
        {
            options ??= new QueryOptions<Permission>();
            options.Includes.Add(p => p.Resource);

            return await base.GetAllAsync(options);
        }
    }
}
