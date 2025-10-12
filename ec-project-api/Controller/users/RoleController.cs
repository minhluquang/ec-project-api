using ec_project_api.Constants.variables;
using ec_project_api.Dtos.response.users;
using ec_project_api.Dtos.response;
using ec_project_api.Facades;
using Microsoft.AspNetCore.Mvc;
using ec_project_api.Constants.Messages;
using ec_project_api.Dtos.request.users;

namespace ec_project_api.Controllers
{
    [Route(PathVariables.RoleRoot)]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RoleFacade _roleFacade;

        public RoleController(RoleFacade roleFacade)
        {
            _roleFacade = roleFacade;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseData<IEnumerable<RoleDto>>>> GetAll([FromQuery] string? statusName)
        {
            try
            {
                var roles = await _roleFacade.GetAllAsync(statusName);
                return Ok(ResponseData<IEnumerable<RoleDto>>.Success(
                    StatusCodes.Status200OK,
                    roles,
                    RoleMessages.RolesRetrievedSuccessfully));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ResponseData<RoleDto>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ResponseData<IEnumerable<RoleDto>>.Error(StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpGet(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<RoleDto>>> GetById(short id)
        {
            try
            {
                var role = await _roleFacade.GetByIdAsync(id);
                return Ok(ResponseData<RoleDto>.Success(
                    StatusCodes.Status200OK,
                    role,
                    RoleMessages.RoleRetrieved));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ResponseData<RoleDto>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ResponseData<RoleDto>.Error(StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ResponseData<bool>>> Create([FromBody] RoleRequest dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, string.Join("; ", errors)));
            }

            try
            {
                var result = await _roleFacade.CreateAsync(dto);
                return StatusCode(StatusCodes.Status201Created,
                    ResponseData<bool>.Success(StatusCodes.Status201Created, result, RoleMessages.RoleCreated));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ResponseData<bool>.Error(StatusCodes.Status409Conflict, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ResponseData<bool>.Error(StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpPut(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<bool>>> Update(short id, [FromBody] RoleRequest dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, string.Join("; ", errors)));
            }

            try
            {
                var result = await _roleFacade.UpdateAsync(id, dto);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, result, RoleMessages.RoleUpdated));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ResponseData<bool>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ResponseData<bool>.Error(StatusCodes.Status409Conflict, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<bool>.Error(
                    StatusCodes.Status400BadRequest,
                    ex.InnerException?.Message ?? ex.Message));
            }
        }

        [HttpDelete(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<bool>>> Delete(short id)
        {
            try
            {
                var result = await _roleFacade.DeleteByIdAsync(id);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, result, RoleMessages.RoleDeleted));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ResponseData<bool>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ResponseData<bool>.Error(StatusCodes.Status409Conflict, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ResponseData<bool>.Error(StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpPost(PathVariables.AssignPermissions)]
        public async Task<ActionResult<ResponseData<object?>>> AssignPermissions(short id, [FromBody] IEnumerable<short> permissionIds)
        {
            try
            {
                await _roleFacade.AssignPermissionsAsync(id, permissionIds);
                return Ok(ResponseData<object?>.Success(
                    StatusCodes.Status200OK,
                    null,
                    RoleMessages.RolePermissionsAssigned));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ResponseData<object?>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ResponseData<object?>.Error(StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
    }
}
