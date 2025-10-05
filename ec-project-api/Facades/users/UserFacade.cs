using AutoMapper;
using ec_project_api.Constants.variables;
using ec_project_api.Constants.Messages;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services;
using ec_project_api.Dtos.Users;
using ec_project_api.Dtos.request.users;

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
            if (users == null || !users.Any())
                throw new KeyNotFoundException(UserMessages.UserNotFound);
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
            }

            if (!request.StatusId.HasValue)
                throw new InvalidOperationException(StatusMessages.StatusRequired);

            var status = await _statusService.GetByIdAsync(request.StatusId.Value, new QueryOptions<Status>
            {
                Filter = s => s.EntityType == EntityVariables.User
            });

            if (status == null)
                throw new KeyNotFoundException(StatusMessages.StatusNotFound);
            if (request.RoleIds != null && request.RoleIds.Any())
            {
                var validRoleIds = (await _roleService.GetAllAsync())
                    .Select(r => r.RoleId)
                    .ToHashSet();

                var invalidRoles = request.RoleIds.Where(rid => !validRoleIds.Contains(rid)).ToList();

                if (invalidRoles.Any())
                {
                    throw new KeyNotFoundException(
                        string.Format(RoleMessages.ListRolesNotFound, string.Join(", ", invalidRoles))
                    );
                }
            }

            var user = _mapper.Map<User>(request);
            var created = await _userService.CreateAsync(user);

            if (created && request.RoleIds != null && request.RoleIds.Any())
            {
                await _userRoleService.AssignRolesAsync(user.UserId, request.RoleIds);
            }

            return created;
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

            if (!request.StatusId.HasValue)
                throw new InvalidOperationException(StatusMessages.StatusRequired);
                
            var status = await _statusService.GetByIdAsync(request.StatusId.Value, new QueryOptions<Status>
            {
                Filter = s => s.EntityType == EntityVariables.User
            });

            if (status == null)
                throw new KeyNotFoundException(StatusMessages.StatusNotFound);

            if (request.RoleIds != null && request.RoleIds.Any())
            {
                var validRoleIds = (await _roleService.GetAllAsync())
                    .Select(r => r.RoleId)
                    .ToHashSet();

                var invalidRoles = request.RoleIds.Where(rid => !validRoleIds.Contains(rid)).ToList();
                if (invalidRoles.Any())
                {
                    throw new KeyNotFoundException(
                        string.Format(RoleMessages.ListRolesNotFound, string.Join(", ", invalidRoles))
                    );
                }
            }

            var updated = await _userService.UpdateAsync(existingUser);

            if (updated && request.RoleIds != null)
            {
                await _userRoleService.AssignRolesAsync(existingUser.UserId, request.RoleIds);
            }

            return updated;
        }

        public async Task<bool> AssignRolesAsync(int userId, IEnumerable<short> roleIds, int? assignedBy = null)
        {
            if (roleIds == null || !roleIds.Any())
                throw new ArgumentException(UserMessages.RoleListEmpty);
            var user = await _userService.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException(UserMessages.UserNotFound);

            if (assignedBy.HasValue)
            {
                var assigner = await _userService.GetByIdAsync(assignedBy.Value);
                if (assigner == null)
                    throw new KeyNotFoundException(UserMessages.AssignerNotFound);
            }

            var validRoleIds = (await _roleService.GetAllAsync())
                .Select(r => r.RoleId)
                .ToHashSet();

            var invalidRoles = roleIds.Where(rid => !validRoleIds.Contains(rid)).ToList();
            if (invalidRoles.Any())
                throw new KeyNotFoundException(string.Format(RoleMessages.ListRolesNotFound, string.Join(", ", invalidRoles)));

            return await _userRoleService.AssignRolesAsync(userId, roleIds, assignedBy);
        }

    }
}
