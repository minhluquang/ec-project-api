using ec_project_api.Constants.variables;
using ec_project_api.Dtos.response.system;
using ec_project_api.Dtos.response;
using ec_project_api.Facades;
using Microsoft.AspNetCore.Mvc;
using ec_project_api.Constants.Messages;

namespace ec_project_api.Controllers
{
    [Route(PathVariables.PermissionRoot)]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly PermissionFacade _permissionFacade;

        public PermissionController(PermissionFacade permissionFacade)
        {
            _permissionFacade = permissionFacade;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseData<IEnumerable<ResourceDto>>>> GetGroupedByResource()
        {
            try
            {
                var result = await _permissionFacade.GetAllGroupedByResourceAsync();

                return Ok(ResponseData<IEnumerable<ResourceDto>>.Success(StatusCodes.Status200OK, result, PermissionMessages.PermissionsRetrievedSuccessfully));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<IEnumerable<ResourceDto>>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }
    }
}
