using System.Security.Claims;
using ec_project_api.Constants.messages;
using ec_project_api.Constants.Messages;
using ec_project_api.Helpers;
using ec_project_api.Services;

namespace ec_project_api.Facades.auth
{
    public class AuthFacade
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly JwtService _jwtService;

        public AuthFacade(IAuthService authService, IUserService userService, JwtService jwtService)
        {
            _authService = authService;
            _userService = userService;
            _jwtService = jwtService;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest dto)
        {
            return await _authService.LoginAsync(dto);
        }

        public async Task<bool> RegisterAsync(RegisterRequest dto)
        {
            var existingUserByUsername = await _authService.FirstOrDefaultAsync(u => u.Username == dto.Username);
            var existingUserByEmail = await _authService.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (existingUserByUsername != null)
                throw new InvalidOperationException(UserMessages.UsernameAlreadyExists);
            if (existingUserByEmail != null)
                throw new InvalidOperationException(UserMessages.EmailAlreadyExists);
            return await _authService.RegisterAsync(dto);
        }

        public async Task<bool> VerifyEmailAsync(string token)
        {
            var principal = _jwtService.ValidateToken(token);
            if (principal == null)
                return false;

            var email = principal.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                throw new InvalidOperationException(AuthMessages.EmailNotFoundInToken);

            var user = await _userService.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                throw new KeyNotFoundException(AuthMessages.UserNotFound);

            if (user.IsVerified)
                throw new InvalidOperationException(AuthMessages.AlreadyVerified);

            user.IsVerified = true;
            Console.WriteLine(user.IsVerified);
            return await _userService.UpdateAsync(user);
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequest dto)
        {
            var user = await _userService.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                throw new KeyNotFoundException(AuthMessages.UserNotFound);

            return await _authService.SendForgotPasswordEmailAsync(user);
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequest dto)
        {
            var principal = _jwtService.ValidateToken(dto.Token);
            if (principal == null)
                throw new InvalidOperationException(JwtMessages.InvalidToken);

            var email = principal.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                throw new InvalidOperationException(AuthMessages.EmailNotFoundInToken);

            var user = await _userService.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                throw new KeyNotFoundException(AuthMessages.UserNotFound);

            user.PasswordHash = PasswordHasher.HashPassword(dto.Password);
            return await _userService.UpdateAsync(user);
        }

    }
}
