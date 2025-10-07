using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.Dtos.request.users;
using ec_project_api.Dtos.Users;
using ec_project_api.Dtos.response;
using ec_project_api.Facades;
using Microsoft.AspNetCore.Mvc;

namespace ec_project_api.Controllers
{
    [Route(PathVariables.UserRoot)]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserFacade _userFacade;

        public UserController(UserFacade userFacade)
        {
            _userFacade = userFacade;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseData<IEnumerable<UserDto>>>> GetAll([FromQuery] UserFilter filter)
        {
            try
            {
                var users = await _userFacade.GetAllAsync(filter);
                return Ok(ResponseData<IEnumerable<UserDto>>.Success(
                    StatusCodes.Status200OK, users, UserMessages.UsersRetrievedSuccessfully));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<IEnumerable<UserDto>>.Error(
                    StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpGet(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<UserDto>>> GetById(int id)
        {
            try
            {
                var user = await _userFacade.GetByIdAsync(id);
                return Ok(ResponseData<UserDto>.Success(StatusCodes.Status200OK, user));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ResponseData<UserDto>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<UserDto>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ResponseData<bool>>> Create([FromBody] UserRequest dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ResponseData<bool>.Error(
                    StatusCodes.Status400BadRequest, string.Join("; ", errors)));
            }

            try
            {
                var created = await _userFacade.CreateAsync(dto);
                return StatusCode(StatusCodes.Status201Created,
                    ResponseData<bool>.Success(StatusCodes.Status201Created, created, UserMessages.UserCreated));
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
        public async Task<ActionResult<ResponseData<bool>>> Update(int id, [FromBody] UserRequest dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ResponseData<bool>.Error(
                    StatusCodes.Status400BadRequest, string.Join("; ", errors)));
            }

            try
            {
                var updated = await _userFacade.UpdateAsync(id, dto);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, updated, UserMessages.UserUpdated));
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

        [HttpPost(PathVariables.AssignRoles)]
        public async Task<ActionResult<ResponseData<bool>>> AssignRoles(
            int userId,
            [FromBody] List<short> roleIds,
            int? assignedBy = null)
        {
            if (roleIds == null || !roleIds.Any())
            {
                return BadRequest(ResponseData<bool>.Error(
                    StatusCodes.Status400BadRequest,
                    UserMessages.RoleListEmpty));
            }

            try
            {
                var result = await _userFacade.AssignRolesAsync(userId, roleIds, assignedBy);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, result));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ResponseData<bool>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpPost("change-password")]
        public async Task<ActionResult<ResponseData<bool>>> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ResponseData<bool>.Error(
                    StatusCodes.Status400BadRequest, string.Join("; ", errors)));
            }

            try
            {
                var result = await _userFacade.ChangePasswordAsync(request);
                return Ok(ResponseData<bool>.Success(
                    StatusCodes.Status200OK, result, UserMessages.PasswordChangedSuccessfully));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ResponseData<bool>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ResponseData<bool>.Error(StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

    }
}
