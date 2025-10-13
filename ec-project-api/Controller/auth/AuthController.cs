using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.Dtos.request.auth;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.auth;
using ec_project_api.Facades.auth;
using Microsoft.AspNetCore.Mvc;

namespace ec_project_api.Controllers
{
    [Route(PathVariables.AuthRoot)]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly AuthFacade _authFacade;

        public AuthController(AuthFacade authFacade, IConfiguration config)
        {
            _config = config;
            _authFacade = authFacade;
        }

        [HttpPost(PathVariables.Login)]
        public async Task<IActionResult> Login([FromBody] LoginRequest dto) =>
            await ExecuteIfValidAsync(dto, async () =>
            {
                var response = await _authFacade.LoginAsync(dto);
                return ResponseData<LoginResponse>.Success(StatusCodes.Status200OK, response, AuthMessages.LoginSuccessful);
            }, handleUnauthorized: true);

        [HttpPost(PathVariables.Register)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest dto) =>
            await ExecuteIfValidAsync(dto, async () =>
            {
                var frontendBase = GetFrontendBaseUrl();
                var result = await _authFacade.RegisterAsync(dto, frontendBase);
                return ResponseData<bool>.Success(StatusCodes.Status201Created, result, AuthMessages.RegisterSuccessful);
            }, successStatusCode: StatusCodes.Status201Created);

        [HttpGet(PathVariables.Verify)]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            var frontendBase = GetFrontendBaseUrl();
            var failedReason = "reason=verify_failed";

            try
            {
                var isVerified = await _authFacade.VerifyEmailAsync(token);
                var redirectPath = isVerified ? PathVariables.Login : $"{PathVariables.Register}?{failedReason}";
                return Redirect($"{frontendBase}/{redirectPath}");
            }
            catch
            {
                return Redirect($"{frontendBase}/{PathVariables.Register}?{failedReason}");
            }
        }

        [HttpPost(PathVariables.ForgotPassword)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest dto) =>
            await ExecuteIfValidAsync(dto, async () =>
            {
                var frontendBase = GetFrontendBaseUrl();
                var result = await _authFacade.ForgotPasswordAsync(dto, frontendBase);
                return ResponseData<bool>.Success(StatusCodes.Status200OK, result, AuthMessages.VerificationEmailSent);
            });

        [HttpPost(PathVariables.ResetPassword)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest dto) =>
            await ExecuteIfValidAsync(dto, async () =>
            {
                var result = await _authFacade.ResetPasswordAsync(dto);
                return ResponseData<bool>.Success(StatusCodes.Status200OK, result, AuthMessages.PasswordResetSuccessful);
            });

        [HttpPost(PathVariables.RefreshToken)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest dto)
        {
            if (string.IsNullOrWhiteSpace(dto.RefreshToken))
                return BadRequest(ResponseData<LoginResponse>.Error(StatusCodes.Status400BadRequest, AuthMessages.MissingRefreshToken));

            return await ExecuteAsync(async () =>
            {
                var response = await _authFacade.RefreshTokenAsync(dto.RefreshToken);
                return ResponseData<RefreshTokenResponse>.Success(StatusCodes.Status200OK, response);
            }, handleUnauthorized: true);
        }

        private async Task<IActionResult> ExecuteIfValidAsync<TDto, TResult>(
            TDto dto,
            Func<Task<ResponseData<TResult>>> action,
            int? successStatusCode = null,
            bool handleUnauthorized = false)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseData<TResult>.Error(StatusCodes.Status400BadRequest, CollectModelErrors()));

            return await ExecuteAsync(action, successStatusCode, handleUnauthorized);
        }

        private async Task<IActionResult> ExecuteAsync<TResult>(
            Func<Task<ResponseData<TResult>>> action,
            int? successStatusCode = null,
            bool handleUnauthorized = false)
        {
            try
            {
                var result = await action();
                return successStatusCode.HasValue
                    ? StatusCode(successStatusCode.Value, result)
                    : Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ResponseData<TResult>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (UnauthorizedAccessException ex) when (handleUnauthorized)
            {
                return Unauthorized(ResponseData<TResult>.Error(StatusCodes.Status401Unauthorized, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ResponseData<TResult>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ResponseData<TResult>.Error(StatusCodes.Status500InternalServerError,
                        ex.InnerException?.Message ?? ex.Message));
            }
        }

        private string CollectModelErrors() =>
            string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));

        private string GetFrontendBaseUrl() =>
            _config["Frontend:BaseUrl"] ?? string.Empty;
    }
}
