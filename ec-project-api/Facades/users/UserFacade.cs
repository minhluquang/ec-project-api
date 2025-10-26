using AutoMapper;
using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.Dtos.Users;
using ec_project_api.Dtos.request.users;
using ec_project_api.Helpers;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services;
using ec_project_api.Dtos.response.pagination;
using System.Security.Claims;

namespace ec_project_api.Facades
{
    public class UserFacade
    {
        private readonly IUserService _userService;
        private readonly IUserRoleService _userRoleService;
        private readonly IRoleService _roleService;
        private readonly IStatusService _statusService;
        private readonly IMapper _mapper;

        public UserFacade(
            IUserService userService,
            IMapper mapper,
            IStatusService statusService,
            IUserRoleService userRoleService,
            IRoleService roleService)
        {
            _userService = userService;
            _userRoleService = userRoleService;
            _roleService = roleService;
            _mapper = mapper;
            _statusService = statusService;
        }

        public async Task<PagedResult<UserDto>> GetAllPagedAsync(UserFilter filter)
        {
            var options = new QueryOptions<User>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Includes = { u => u.Status, u => u.UserRoleDetails }
            };

            options.Filter = BuildUserFilter(filter);

            var pagedUsers = await _userService.GetAllPagedAsync(options);

            if (pagedUsers == null || !pagedUsers.Items.Any())
                throw new KeyNotFoundException(UserMessages.UserNotFound);
            var dtoList = _mapper.Map<IEnumerable<UserDto>>(pagedUsers.Items);
            var pagedResultDto = new PagedResult<UserDto>
            {
                Items = dtoList,
                TotalCount = pagedUsers.TotalCount,
                TotalPages = pagedUsers.TotalPages,
                PageNumber = pagedUsers.PageNumber,
                PageSize = pagedUsers.PageSize
            };

            return pagedResultDto;
        }

        public async Task<UserDto> GetByIdAsync(int id)
        {
            var user = await _userService.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(UserMessages.UserNotFound);

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> GetCurrentUserAsync(ClaimsPrincipal userPrincipal)
        {
            if (userPrincipal == null)
                throw new UnauthorizedAccessException(UserMessages.UserNotFound);

            var userIdClaim = userPrincipal.FindFirst("UserId")
                              ?? userPrincipal.FindFirst(ClaimTypes.NameIdentifier)
                              ?? userPrincipal.FindFirst(ClaimTypes.Name);

            if (userIdClaim == null)
                throw new UnauthorizedAccessException(UserMessages.UserNotFound);

            if (!int.TryParse(userIdClaim.Value, out var userId))
                throw new InvalidOperationException(AuthMessages.InvalidOrExpiredToken);

            var user = await GetByIdAsync(userId);
            return user;
        }

        public async Task<bool> CreateAsync(UserRequest request)
        {
            await EnsureUserUniqueAsync(request);
            await EnsureStatusValidAsync(request.StatusId);
            await EnsureRoleIdsValidAsync(request.RoleIds);

            var user = _mapper.Map<User>(request);
            var created = await _userService.CreateAsync(user);

            if (created && request.RoleIds?.Any() == true)
                await _userRoleService.AssignRolesAsync(user.UserId, request.RoleIds);

            return created;
        }

        public async Task<bool> UpdateAsync(int id, UserRequest request)
        {
            var user = await _userService.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(UserMessages.UserNotFound);

            await EnsureUserUniqueAsync(request, id);
            await EnsureStatusValidAsync(request.StatusId);
            await EnsureRoleIdsValidAsync(request.RoleIds);

            _mapper.Map(request, user);

            var updated = await _userService.UpdateAsync(user);

            if (updated && request.RoleIds != null)
                await _userRoleService.AssignRolesAsync(user.UserId, request.RoleIds);

            return updated;
        }

        public async Task<bool> AssignRolesAsync(int userId, IEnumerable<short> roleIds, int? assignedBy = null)
        {
            if (roleIds == null || !roleIds.Any())
                throw new ArgumentException(UserMessages.RoleListEmpty);

            var user = await _userService.GetByIdAsync(userId)
                ?? throw new KeyNotFoundException(UserMessages.UserNotFound);

            if (assignedBy.HasValue)
            {
                var assigner = await _userService.GetByIdAsync(assignedBy.Value);
                if (assigner == null)
                    throw new KeyNotFoundException(UserMessages.AssignerNotFound);
            }

            await EnsureRoleIdsValidAsync(roleIds);

            return await _userRoleService.AssignRolesAsync(userId, roleIds, assignedBy);
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordRequest request)
        {
            var user = await _userService.GetByIdAsync(request.UserId)
                ?? throw new KeyNotFoundException(UserMessages.UserNotFound);

            if (request.NewPassword != request.ConfirmPassword)
                throw new InvalidOperationException(UserMessages.PasswordsDoNotMatch);

            if (string.IsNullOrEmpty(user.PasswordHash))
                throw new InvalidOperationException(GeneralMessages.Invalid);

            if (!PasswordHasher.VerifyPassword(request.OldPassword, user.PasswordHash))
                throw new InvalidOperationException(UserMessages.OldPasswordIncorrect);

            user.PasswordHash = PasswordHasher.HashPassword(request.NewPassword);

            var updated = await _userService.UpdateAsync(user);
            if (!updated)
                throw new InvalidOperationException(UserMessages.PasswordNotChanged);

            return true;
        }

        private static System.Linq.Expressions.Expression<Func<User, bool>> BuildUserFilter(UserFilter filter)
        {
            return u =>
                (string.IsNullOrEmpty(filter.StatusName) ||
                    (u.Status != null && u.Status.Name == filter.StatusName && u.Status.EntityType == EntityVariables.User)) &&
                (string.IsNullOrEmpty(filter.Search) ||
                    u.Username.Contains(filter.Search) ||
                    u.Email.Contains(filter.Search) ||
                    (u.FullName != null && u.FullName.Contains(filter.Search))) &&
                (string.IsNullOrEmpty(filter.Phone) ||
                    (u.Phone != null && u.Phone.Contains(filter.Phone))) &&
                (!filter.HasRole.HasValue ||
                    (filter.HasRole.Value ? u.UserRoleDetails.Any() : !u.UserRoleDetails.Any()));
        }

        private async Task EnsureUserUniqueAsync(UserRequest request, int? excludeUserId = null)
        {
            var existing = await _userService.FirstOrDefaultAsync(
                u =>
                    (u.Email == request.Email ||
                     u.Username == request.Username ||
                     (!string.IsNullOrEmpty(request.Phone) && u.Phone == request.Phone))
                    && (!excludeUserId.HasValue || u.UserId != excludeUserId.Value)
            );

            if (existing != null)
            {
                if (existing.Email == request.Email)
                    throw new InvalidOperationException(UserMessages.EmailAlreadyExists);
                if (existing.Username == request.Username)
                    throw new InvalidOperationException(UserMessages.UsernameAlreadyExists);
                if (!string.IsNullOrEmpty(request.Phone) && existing.Phone == request.Phone)
                    throw new InvalidOperationException(UserMessages.PhoneAlreadyExists);
            }
        }

        private async Task EnsureStatusValidAsync(short? statusId)
        {
            if (!statusId.HasValue)
                throw new InvalidOperationException(StatusMessages.StatusRequired);

            var status = await _statusService.GetByIdAsync(statusId.Value, new QueryOptions<Status>
            {
                Filter = s => s.EntityType == EntityVariables.User
            });

            if (status == null)
                throw new KeyNotFoundException(StatusMessages.StatusNotFound);
        }

        private async Task EnsureRoleIdsValidAsync(IEnumerable<short>? roleIds)
        {
            if (roleIds == null || !roleIds.Any()) return;

            var validRoleIds = (await _roleService.GetAllAsync())
                .Select(r => r.RoleId)
                .ToHashSet();

            var invalidRoles = roleIds.Where(rid => !validRoleIds.Contains(rid)).ToList();

            if (invalidRoles.Any())
                throw new KeyNotFoundException(
                    string.Format(RoleMessages.ListRolesNotFound, string.Join(", ", invalidRoles))
                );
        }
    }
}
