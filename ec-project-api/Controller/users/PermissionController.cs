using ec_project_api.Constants.variables;
using ec_project_api.Constants.Messages;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.system;
using ec_project_api.Facades;
using ec_project_api.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace ec_project_api.Controllers
{
    [Route(PathVariables.PermissionRoot)]
    public class PermissionController : BaseController
    {
        private readonly PermissionFacade _permissionFacade;

        public PermissionController(PermissionFacade permissionFacade)
        {
            _permissionFacade = permissionFacade;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseData<IEnumerable<ResourceDto>>>> GetGroupedByResource()
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _permissionFacade.GetAllGroupedByResourceAsync();
                return ResponseData<IEnumerable<ResourceDto>>.Success(StatusCodes.Status200OK, result, PermissionMessages.PermissionsRetrievedSuccessfully);
            });
        }
    }
}
