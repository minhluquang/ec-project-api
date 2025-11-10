using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.Dtos.request.users;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.users;
using ec_project_api.Facades;
using ec_project_api.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ec_project_api.Controllers
{
    [Route(PathVariables.RoleRoot)]
    public class RoleController : BaseController
    {
        private readonly RoleFacade _roleFacade;

        public RoleController(RoleFacade roleFacade)
        {
            _roleFacade = roleFacade;
        }

        [HttpGet]
        [Authorize(Policy = "Role.GetAll")]
        public async Task<ActionResult<ResponseData<IEnumerable<RoleDto>>>> GetAll([FromQuery] string? statusName)
        {
            return await ExecuteAsync(async () =>
            {
                var roles = await _roleFacade.GetAllAsync(statusName);
                return ResponseData<IEnumerable<RoleDto>>.Success(StatusCodes.Status200OK, roles, RoleMessages.RolesRetrievedSuccessfully);
            });
        }

        [HttpGet(PathVariables.GetById)]
        [Authorize(Policy = "Role.GetById")]
        public async Task<ActionResult<ResponseData<RoleDto>>> GetById(short id)
        {
            return await ExecuteAsync(async () =>
            {
                var role = await _roleFacade.GetByIdAsync(id);
                return ResponseData<RoleDto>.Success(StatusCodes.Status200OK, role, RoleMessages.RoleRetrieved);
            });
        }

        [HttpPost]
        public async Task<ActionResult<ResponseData<bool>>> Create([FromBody] RoleRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, GetModelErrors()));

            return await ExecuteAsync(async () =>
            {
                var result = await _roleFacade.CreateAsync(dto);
                return ResponseData<bool>.Success(StatusCodes.Status201Created, result, RoleMessages.RoleCreated);
            }, 201);
        }

        [HttpPut(PathVariables.GetById)]
        [Authorize(Policy = "Role.Update")]
        public async Task<ActionResult<ResponseData<bool>>> Update(short id, [FromBody] RoleRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, GetModelErrors()));

            return await ExecuteAsync(async () =>
            {
                var result = await _roleFacade.UpdateAsync(id, dto);
                return ResponseData<bool>.Success(StatusCodes.Status200OK, result, RoleMessages.RoleUpdated);
            });
        }

        [HttpDelete(PathVariables.GetById)]
        [Authorize(Policy = "Role.Delete")]
        public async Task<ActionResult<ResponseData<bool>>> Delete(short id)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _roleFacade.DeleteByIdAsync(id);
                return ResponseData<bool>.Success(StatusCodes.Status200OK, result, RoleMessages.RoleDeleted);
            });
        }

        [HttpPost(PathVariables.AssignPermissions)]
        [Authorize(Policy = "Role.AddPermission")]
        public async Task<ActionResult<ResponseData<object?>>> AssignPermissions(short id, [FromBody] IEnumerable<short> permissionIds)
        {
            return await ExecuteAsync(async () =>
            {
                await _roleFacade.AssignPermissionsAsync(id, permissionIds);
                return ResponseData<object?>.Success(StatusCodes.Status200OK, null, RoleMessages.RolePermissionsAssigned);
            });
        }
    }
}
