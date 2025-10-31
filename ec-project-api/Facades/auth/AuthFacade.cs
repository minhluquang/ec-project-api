using System.Security.Claims;
using ec_project_api.Constants.messages;
using ec_project_api.Constants.Messages;
using ec_project_api.Dtos.response.auth;
using ec_project_api.Helpers;
using ec_project_api.Models;
using ec_project_api.Services;

namespace ec_project_api.Facades.auth
{
    public class AuthFacade
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly JwtService _jwtService;

        public AuthFacade(
            IAuthService authService,
            IUserService userService,
            JwtService jwtService)
        {
            _authService = authService;
            _userService = userService;
            _jwtService = jwtService;
        }

        public Task<LoginResponse> LoginAsync(LoginRequest dto)
            => _authService.LoginAsync(dto);

        public async Task<bool> RegisterAsync(RegisterRequest dto, string baseUrl)
        {
            await EnsureUserDoesNotExist(dto);
            return await _authService.RegisterAsync(dto, baseUrl);
        }

        public async Task<bool> VerifyEmailAsync(string token)
        {
            var email = ExtractEmailFromToken(token);
            var user = await GetUserByEmailAsync(email);

            if (user.IsVerified)
                throw new InvalidOperationException(AuthMessages.AlreadyVerified);

            user.IsVerified = true;
            return await _userService.UpdateAsync(user);
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequest dto, string baseUrl)
        {
            var user = await GetUserByEmailAsync(dto.Email);
            return await _authService.SendForgotPasswordEmailAsync(user, baseUrl);
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequest dto)
        {
            var email = ExtractEmailFromToken(dto.Token);
            var user = await GetUserByEmailAsync(email);

            user.PasswordHash = PasswordHasher.HashPassword(dto.Password);
            return await _userService.UpdateAsync(user);
        }

        public async Task<RefreshTokenResponse> RefreshTokenAsync(string refreshToken)
        {
            var principal = _jwtService.ValidateToken(refreshToken)
                ?? throw new UnauthorizedAccessException(AuthMessages.InvalidOrExpiredToken);

            var userId = ExtractUserId(principal);
            var user = await _userService.FirstOrDefaultAsync(u => u.UserId == userId)
                ?? throw new KeyNotFoundException(UserMessages.UserNotFound);

            return await _authService.BuildRefreshTokenResponse(user);
        }

        private async Task EnsureUserDoesNotExist(RegisterRequest dto)
        {
            if (await _authService.FirstOrDefaultAsync(u => u.Username == dto.Username) is not null)
                throw new InvalidOperationException(UserMessages.UsernameAlreadyExists);

            if (await _authService.FirstOrDefaultAsync(u => u.Email == dto.Email) is not null)
                throw new InvalidOperationException(UserMessages.EmailAlreadyExists);
        }

        private string ExtractEmailFromToken(string token)
        {
            var principal = _jwtService.ValidateToken(token)
                ?? throw new InvalidOperationException(AuthMessages.InvalidOrExpiredToken);

            var email = principal.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                throw new InvalidOperationException(AuthMessages.EmailNotFoundInToken);

            return email;
        }

        private int ExtractUserId(ClaimsPrincipal principal)
        {
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException(UserMessages.UserNotFound);

            return userId;
        }

        private async Task<User> GetUserByEmailAsync(string email)
        {
            var user = await _userService.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                throw new KeyNotFoundException(AuthMessages.UserNotFound);
            return user;
        }
    }
}
