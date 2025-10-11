using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.Dtos.response;
using ec_project_api.Facades.auth;
using ec_project_api.Models;
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
        public async Task<IActionResult> Login([FromBody] LoginRequest dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ResponseData<User>.Error(StatusCodes.Status400BadRequest, string.Join("; ", errors)));
            }
            try
            {
                var response = await _authFacade.LoginAsync(dto);
                return Ok(ResponseData<LoginResponse>.Success(StatusCodes.Status200OK, response, AuthMessages.LoginSuccessful));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ResponseData<LoginResponse>.Error(
                    StatusCodes.Status404NotFound,
                    ex.Message
                ));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ResponseData<LoginResponse>.Error(
                    StatusCodes.Status401Unauthorized,
                    ex.Message
                ));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ResponseData<LoginResponse>.Error(
                    StatusCodes.Status400BadRequest,
                    ex.Message
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ResponseData<LoginResponse>.Error(
                        StatusCodes.Status500InternalServerError,
                        ex.Message
                    )
                );
            }
        }

        [HttpPost(PathVariables.Register)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, string.Join("; ", errors)));
            }

            try
            {
                var user = await _authFacade.RegisterAsync(dto);
                return StatusCode(StatusCodes.Status201Created,
                    ResponseData<bool>.Success(StatusCodes.Status201Created, user, AuthMessages.RegisterSuccessful));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ResponseData<bool>.Error(StatusCodes.Status409Conflict, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ResponseData<bool>.Error(StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpGet(PathVariables.Verify)]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            try
            {
                var result = await _authFacade.VerifyEmailAsync(token);

                var frontendBase = _config["Frontend:BaseUrl"];
                string redirectUrl;

                if (result)
                {
                    redirectUrl = $"{frontendBase}/{PathVariables.Login}";
                }
                else
                {
                    redirectUrl = $"{frontendBase}/{PathVariables.Register}?reason=verify_failed";
                }
                Console.WriteLine(redirectUrl);
                return Redirect(redirectUrl);
            }
            catch
            {
                var frontendBase = _config["Frontend:BaseUrl"];
                var redirectUrl = $"{frontendBase}/{PathVariables.Register}?reason=verify_failed";
                return Redirect(redirectUrl);
            }
        }

        [HttpPost(PathVariables.ForgotPassword)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, string.Join("; ", errors)));
            }

            try
            {
                var result = await _authFacade.ForgotPasswordAsync(dto);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, result, AuthMessages.VerificationEmailSent));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ResponseData<bool>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ResponseData<bool>.Error(StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpPost(PathVariables.ResetPassword)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, string.Join("; ", errors)));
            }

            try
            {
                var result = await _authFacade.ResetPasswordAsync(dto);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, result, AuthMessages.PasswordResetSuccessful));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ResponseData<bool>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ResponseData<bool>.Error(StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

    }

}