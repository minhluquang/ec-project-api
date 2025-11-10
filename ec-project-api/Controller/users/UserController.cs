using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.Dtos.Users;
using ec_project_api.Dtos.request.users;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.pagination;
using ec_project_api.Facades;
using ec_project_api.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ec_project_api.Controllers
{
    [Route(PathVariables.UserRoot)]
    public class UserController : BaseController
    {
        private readonly UserFacade _userFacade;

        public UserController(UserFacade userFacade)
        {
            _userFacade = userFacade;
        }

        [HttpGet]
        [Authorize(Policy = "User.GetAll")]
        public async Task<ActionResult<ResponseData<PagedResult<UserDto>>>> GetAll([FromQuery] UserFilter filter)
        {
            return await ExecuteAsync(async () =>
            {
                var users = await _userFacade.GetAllPagedAsync(filter);
                return ResponseData<PagedResult<UserDto>>.Success(StatusCodes.Status200OK, users, UserMessages.UsersRetrievedSuccessfully);
            });
        }

        [HttpGet(PathVariables.GetById)]
        [Authorize(Policy = "User.GetById")]
        public async Task<ActionResult<ResponseData<UserDto>>> GetById(int id)
        {
            return await ExecuteAsync(async () =>
            {
                var user = await _userFacade.GetByIdAsync(id);
                return ResponseData<UserDto>.Success(StatusCodes.Status200OK, user);
            });
        }

        [HttpPost]
        [Authorize(Policy = "User.Create")]
        public async Task<ActionResult<ResponseData<bool>>> Create([FromBody] UserRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, GetModelErrors()));

            return await ExecuteAsync(async () =>
            {
                var created = await _userFacade.CreateAsync(dto);
                return ResponseData<bool>.Success(StatusCodes.Status201Created, created, UserMessages.UserCreated);
            }, 201);
        }

        [HttpPut(PathVariables.GetById)]
        [Authorize(Policy = "User.Update")]
        public async Task<ActionResult<ResponseData<bool>>> Update(int id, [FromBody] UserRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, GetModelErrors()));
            return await ExecuteAsync(async () =>
            {
                var updated = await _userFacade.UpdateAsync(id, dto);
                return ResponseData<bool>.Success(StatusCodes.Status200OK, updated, UserMessages.UserUpdated);
            });
        }

        [HttpPost(PathVariables.AssignRoles)]
        [Authorize(Policy = "User.AssignRole")]
        public async Task<ActionResult<ResponseData<bool>>> AssignRoles(int userId, [FromBody] List<short> roleIds, int? assignedBy = null)
        {
            if (roleIds == null || !roleIds.Any())
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, UserMessages.RoleListEmpty));

            return await ExecuteAsync(async () =>
            {
                var result = await _userFacade.AssignRolesAsync(userId, roleIds, assignedBy);
                return ResponseData<bool>.Success(StatusCodes.Status200OK, result);
            });
        }

        [HttpPost(PathVariables.ChangePassword)]
        public async Task<ActionResult<ResponseData<bool>>> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, GetModelErrors()));

            return await ExecuteAsync(async () =>
            {
                var result = await _userFacade.ChangePasswordAsync(request);
                return ResponseData<bool>.Success(StatusCodes.Status200OK, result, UserMessages.PasswordChangedSuccessfully);
            });
        }

        [HttpGet(PathVariables.UserProfile)]
        public async Task<ActionResult<ResponseData<UserDto>>> GetCurrentUser()
        {
            return await ExecuteAsync(async () =>
            {
                var user = await _userFacade.GetCurrentUserAsync(User);
                return ResponseData<UserDto>.Success(StatusCodes.Status200OK, user);
            });
        }

        [HttpPatch(PathVariables.UploadAvatarImage)]
        public async Task<ActionResult<ResponseData<bool>>> UploadAvatarImage([FromForm] UserAvatarImageRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, GetModelErrors()));

            return await ExecuteAsync(async () =>
            {
                var user = await _userFacade.UploadAvatarImageAsync(User,request);
                return ResponseData<bool>.Success(StatusCodes.Status200OK, user);
            });
        }

        [HttpDelete(PathVariables.DeleteAvatarImage)]
        public async Task<ActionResult<ResponseData<bool>>> DeleteAvatarImage()
        {
            return await ExecuteAsync(async () =>
            {
                var user = await _userFacade.DeleteAvatarImageAsync(User);
                return ResponseData<bool>.Success(StatusCodes.Status200OK, user);
            });
        }
        
    }
}
