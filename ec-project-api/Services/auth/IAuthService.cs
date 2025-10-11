using ec_project_api.Dtos.response.auth;
using ec_project_api.Models;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services
{
    public interface IAuthService : IBaseService<User, int>
    {
        Task<LoginResponse> LoginAsync(LoginRequest dto);
        Task<bool> RegisterAsync(RegisterRequest dto);
        Task<bool> SendForgotPasswordEmailAsync(User user);
        Task<RefreshTokenResponse> BuildRefreshTokenResponse(User user);
    }
}
