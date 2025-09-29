using AutoMapper;
using ec_project_api.Constants.variables;
using ec_project_api.Constants.Messages;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services;
using ec_project_api.Dtos.Users;
using ec_project_api.Dtos.request.users;
using ec_project_api.Helpers;

namespace ec_project_api.Facades
{
    public class UserFacade
    {
        private readonly IUserService _userService;
        private readonly IUserRoleService _userRoleService;
        private readonly IRoleService _roleService;
        private readonly IStatusService _statusService;
        private readonly IMapper _mapper;

        public UserFacade(IUserService userService, IMapper mapper, IStatusService statusService, IUserRoleService userRoleService, IRoleService roleService)
        {
            _userService = userService;
            _userRoleService = userRoleService;
            _roleService = roleService;
            _mapper = mapper;
            _statusService = statusService;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync(UserFilter filter)
        {
            var options = new QueryOptions<User>
            {
                Includes = { u => u.Status, u => u.UserRoleDetails }
            };

            options.Filter = u =>
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

            var users = await _userService.GetAllAsync(options);
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto> GetByIdAsync(int id)
        {
            var user = await _userService.GetByIdAsync(id);

            if (user == null)
                throw new KeyNotFoundException(UserMessages.UserNotFound);

            return _mapper.Map<UserDto>(user);
        }

        public async Task<bool> CreateAsync(UserRequest request)
        {
            var existingUser = await _userService.FirstOrDefaultAsync(
                u => u.Email == request.Email
                  || u.Username == request.Username
                  || (u.Phone != null && u.Phone == request.Phone)
            );

            if (existingUser != null)
            {
                if (existingUser.Email == request.Email)
                    throw new InvalidOperationException(UserMessages.EmailAlreadyExists);

                if (existingUser.Username == request.Username)
                    throw new InvalidOperationException(UserMessages.UsernameAlreadyExists);

                if (!string.IsNullOrEmpty(request.Phone) && existingUser.Phone == request.Phone)
                    throw new InvalidOperationException(UserMessages.PhoneAlreadyExists);

                throw new InvalidOperationException(UserMessages.UsernameAlreadyExists);
            }

            var user = _mapper.Map<User>(request);
            user.PasswordHash = PasswordHasher.HashPassword(request.PasswordHash);

            await _statusService.GetByIdAsync(user.StatusId, new QueryOptions<Status>
            {
                Filter = s => s.EntityType == EntityVariables.User
            });

            return await _userService.CreateAsync(user);
        }

        public async Task<bool> UpdateAsync(int id, UserRequest request)
        {
            var existingUser = await _userService.GetByIdAsync(id);

            if (existingUser == null)
                throw new KeyNotFoundException(UserMessages.UserNotFound);

            if (!string.IsNullOrEmpty(request.Email) && request.Email != existingUser.Email)
            {
                var emailDuplicate = await _userService.FirstOrDefaultAsync(
                    u => u.Email == request.Email && u.UserId != id
                );
                if (emailDuplicate != null)
                    throw new InvalidOperationException(UserMessages.EmailAlreadyExists);
            }

            if (!string.IsNullOrEmpty(request.Username) && request.Username != existingUser.Username)
            {
                var usernameDuplicate = await _userService.FirstOrDefaultAsync(
                    u => u.Username == request.Username && u.UserId != id
                );
                if (usernameDuplicate != null)
                    throw new InvalidOperationException(UserMessages.UsernameAlreadyExists);
            }

            if (!string.IsNullOrEmpty(request.Phone) && request.Phone != existingUser.Phone)
            {
                var phoneDuplicate = await _userService.FirstOrDefaultAsync(
                    u => u.Phone == request.Phone && u.UserId != id
                );
                if (phoneDuplicate != null)
                    throw new InvalidOperationException(UserMessages.PhoneAlreadyExists);
            }

            _mapper.Map(request, existingUser);

            if (!string.IsNullOrEmpty(request.PasswordHash))
            {
                existingUser.PasswordHash = PasswordHasher.HashPassword(request.PasswordHash);
            }

            await _statusService.GetByIdAsync(existingUser.StatusId, new QueryOptions<Status>
            {
                Filter = s => s.EntityType == EntityVariables.User
            });

            return await _userService.UpdateAsync(existingUser);
        }

        public async Task<bool> AssignRolesAsync(int userId, IEnumerable<short> roleIds, int? assignedBy = null)
        {
            if (roleIds == null || !roleIds.Any())
                throw new ArgumentException(UserMessages.RoleListEmpty);

            var validRoleIds = (await _roleService.GetAllAsync())
                .Select(r => r.RoleId)
                .ToHashSet();

            var invalidRoles = roleIds.Where(rid => !validRoleIds.Contains(rid)).ToList();
            if (invalidRoles.Any())
                throw new KeyNotFoundException(string.Format(RoleMessages.RolesNotFound, string.Join(", ", invalidRoles)));

            return await _userRoleService.AssignRolesAsync(userId, roleIds, assignedBy);
        }


    }
}
