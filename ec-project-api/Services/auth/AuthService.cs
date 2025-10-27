using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.Dtos.response.auth;
using ec_project_api.Helpers;
using ec_project_api.Interfaces.Users;
using ec_project_api.Models;
using ec_project_api.Services.Bases;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ec_project_api.Services
{
    public class AuthService : BaseService<User, int>, IAuthService
    {
        private readonly JwtService _jwtService;
        private readonly CustomUserService _customUserService;
        private readonly CustomEmailService _customEmailService;
        private readonly IStatusService _statusService;

        public AuthService(
            IUserRepository userRepository,
            JwtService jwtService,
            CustomUserService customUserService,
            CustomEmailService customEmailService,
            IStatusService statusService
        ) : base(userRepository)
        {
            _jwtService = jwtService;
            _customUserService = customUserService;
            _customEmailService = customEmailService;
            _statusService = statusService;
        }
        public async Task<LoginResponse> LoginAsync(LoginRequest dto)
        {
            var user = await ValidateUserCredentials(dto);
            await ValidateUserStatusAsync(user);

            var identity = await _customUserService.BuildClaimsIdentityAsync(user.Username);
            var accessToken = _jwtService.GenerateToken(identity);
            var refreshToken = _jwtService.GenerateRefreshToken(identity);

            return new LoginResponse
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                RefreshTokenExpiry = _jwtService.GetRefreshTokenExpiryDate()
            };
        }

        public async Task<bool> RegisterAsync(RegisterRequest dto, string baseUrl)
        {
            var status = await _statusService.FirstOrDefaultAsync(s => s.Name == StatusVariables.Active && s.EntityType == EntityVariables.User);
            if (status == null)
                throw new KeyNotFoundException(StatusMessages.StatusNotFound);
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = PasswordHasher.HashPassword(dto.Password),
                StatusId = status.StatusId
            };

            await _repository.AddAsync(user);
            await _repository.SaveChangesAsync();

            return await EmailWorkflowHelper.SendEmailWithTokenAsync(
                _customEmailService,
                _jwtService,
                user,
                baseUrl,
                EmailType.Verification
            );
        }

        public async Task<bool> SendForgotPasswordEmailAsync(User user, string baseUrl)
            => await EmailWorkflowHelper.SendEmailWithTokenAsync(
                _customEmailService,
                _jwtService,
                user,
                baseUrl,
                EmailType.PasswordReset
            );

        public async Task<RefreshTokenResponse> BuildRefreshTokenResponse(User user)
        {
            var identity = await _customUserService.BuildClaimsIdentityAsync(user.Username);
            var token = _jwtService.GenerateToken(identity);

            return new RefreshTokenResponse { Token = token };
        }

        private async Task<User> ValidateUserCredentials(LoginRequest dto)
        {
            var user = await _repository.FirstOrDefaultAsync(u => u.Username == dto.Username);

            if (user == null || string.IsNullOrEmpty(user.PasswordHash) ||
                !PasswordHasher.VerifyPassword(dto.Password, user.PasswordHash))
            {
                throw new KeyNotFoundException(AuthMessages.InvalidCredentials);
            }

            if (!user.IsVerified)
            {
                throw new BadHttpRequestException(AuthMessages.NotVerified);
            }
            return user;
        }

        private static async Task ValidateUserStatusAsync(User user)
        {
            switch (user.Status.Name)
            {
                case StatusVariables.Lock:
                    throw new UnauthorizedAccessException(AuthMessages.AccountLocked);
                case StatusVariables.Inactive:
                    throw new InvalidOperationException(AuthMessages.AccountInactive);
                default:
                    break;
            }

            await Task.CompletedTask;
        }
    }
}
