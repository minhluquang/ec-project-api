using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.Helpers;
using ec_project_api.Interfaces.Users;
using ec_project_api.Models;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services
{
    public class AuthService : BaseService<User, int>, IAuthService
    {
        private readonly JwtService _jwtService;
        private readonly CustomUserService _customUserService;
        private readonly CustomEmailService _customEmailService;
        private readonly IConfiguration _config;

        public AuthService(
            IUserRepository userRepository,
            JwtService jwtService,
            CustomUserService customUserService,
            IConfiguration config,
            CustomEmailService customEmailService) : base(userRepository)
        {
            _jwtService = jwtService;
            _customUserService = customUserService;
            _customEmailService = customEmailService;
            _config = config;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest dto)
        {
            var user = await _repository.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null || string.IsNullOrEmpty(user.PasswordHash) || !PasswordHasher.VerifyPassword(dto.Password, user.PasswordHash))
                throw new KeyNotFoundException(AuthMessages.InvalidCredentials);

            var identity = await _customUserService.BuildClaimsIdentityAsync(user.Username);
            var accessToken = _jwtService.GenerateToken(identity);
            var refreshToken = _jwtService.GenerateRefreshToken(identity);
            var refreshExpiry = _jwtService.GetRefreshTokenExpiryDate();

            return new LoginResponse
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                RefreshTokenExpiry = refreshExpiry
            };
        }

        public async Task<bool> RegisterAsync(RegisterRequest dto)
        {
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = PasswordHasher.HashPassword(dto.Password),
                StatusId = 1
            };

            await _repository.AddAsync(user);
            await _repository.SaveChangesAsync();

            var verifyToken = _jwtService.GenerateEmailVerificationToken(user.Email);
            var verifyUrl = $"{_config["App:BaseUrl"]}{PathVariables.AuthRoot}/{PathVariables.Verify}?token={verifyToken}";

            var subject = "Xác nhận tài khoản EC Project";
            var body = $@"
            <h3>Xin chào {user.Username},</h3>
            <p>Bạn vui lòng xác nhận tài khoản của mình bằng cách nhấn vào liên kết dưới đây:</p>
            <p><a href='{verifyUrl}' style='color:#2d89ef;font-weight:bold;'>Xác nhận tài khoản</a></p>
            <p>Liên kết này sẽ hết hạn sau <b>5 phút</b>.</p>";

            try
            {
                await _customEmailService.SendEmailAsync(user.Email, subject, body);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
