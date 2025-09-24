using ec_project_api.Constants;
using ec_project_api.Dtos.response.users;
using ec_project_api.Dtos.response;
using ec_project_api.Facades;
using Microsoft.AspNetCore.Mvc;
using ec_project_api.Constants.Messages;

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
        public async Task<ActionResult<ResponseData<IEnumerable<RoleDto>>>> GetAll()
        {
            try
            {
                var roles = await _roleFacade.GetAllAsync();
                return Ok(ResponseData<IEnumerable<RoleDto>>.Success(StatusCodes.Status200OK, roles));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<IEnumerable<RoleDto>>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpGet(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<RoleDto>>> GetById(short id)
        {
            try
            {
                var role = await _roleFacade.GetByIdAsync(id);
                if (role == null)
                    return NotFound(ResponseData<RoleDto>.Error(StatusCodes.Status404NotFound, RoleMessages.RoleNotFound));

                return Ok(ResponseData<RoleDto>.Success(StatusCodes.Status200OK, role));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<RoleDto>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ResponseData<RoleDto>>> Create([FromBody] RoleRequest dto)
        {
            try
            {
                var createdRole = await _roleFacade.CreateAsync(dto);
                return Ok(ResponseData<RoleDto>.Success(StatusCodes.Status201Created, createdRole, RoleMessages.RoleCreated));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<RoleDto>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpPut(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<bool>>> Update(short id, [FromBody] RoleRequest dto)
        {
            try
            {
                var success = await _roleFacade.UpdateAsync(id, dto);
                if (!success)
                    return NotFound(ResponseData<bool>.Error(StatusCodes.Status404NotFound, RoleMessages.RoleNotFound));

                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, true));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpDelete(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<bool>>> Delete(short id)
        {
            try
            {
                var success = await _roleFacade.DeleteByIdAsync(id);
                if (!success)
                    return NotFound(ResponseData<bool>.Error(StatusCodes.Status404NotFound, RoleMessages.RoleNotFound));

                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, true));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpPost(PathVariables.AssignPermissions)]
        public async Task<ActionResult<ResponseData<bool>>> AssignPermissions(short id, [FromBody] IEnumerable<short> permissionIds)
        {
            try
            {
                var success = await _roleFacade.AssignPermissionsAsync(id, permissionIds);
                if (!success)
                    return NotFound(ResponseData<bool>.Error(StatusCodes.Status404NotFound, RoleMessages.RoleNotFoundOrPermissionsInvalid));

                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, true));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }
    }
}
