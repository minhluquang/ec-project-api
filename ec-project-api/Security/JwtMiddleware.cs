using System.Security.Claims;
using ec_project_api.Constants.messages;
using ec_project_api.Constants.variables;
using Microsoft.AspNetCore.Authorization;

namespace ec_project_api.Security
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtService _jwtService;
        private readonly IServiceScopeFactory _scopeFactory;

        private static readonly string[] WhitelistPaths = new[]
        {
            PathVariables.AuthRoot,
            PathVariables.SwaggerPath,
            PathVariables.SwaggerIndex,
            PathVariables.SwaggerJson,
            PathVariables.Health
        };

        public JwtMiddleware(RequestDelegate next, JwtService jwtService, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _jwtService = jwtService;
            _scopeFactory = scopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower() ?? "";

            bool isWhitelisted = WhitelistPaths.Any(p => 
                path.StartsWith(p.ToLower(), StringComparison.OrdinalIgnoreCase));

            var endpoint = context.GetEndpoint();
            var allowAnonymous = endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null;

            if (!isWhitelisted && !allowAnonymous)
            {
                /*var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                if (string.IsNullOrEmpty(token))
                {
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(new
                    {
                        message = JwtMessages.TokenNotProvided,
                        error = "Unauthorized",
                        status = 401
                    });
                    return;
                }

                var isValid = await AttachUserToContext(context, token);
                
                if (!isValid)
                {
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(new 
                    { 
                        message = JwtMessages.InvalidToken,
                        error = "Unauthorized",
                        status = 401
                    });
                    return;
                }*/
            }
            else if (isWhitelisted || allowAnonymous)
            {
                var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                
                if (!string.IsNullOrEmpty(token))
                {
                    await AttachUserToContext(context, token);
                }
            }

            await _next(context);
        }

        private async Task<bool> AttachUserToContext(HttpContext context, string token)
        {
            try
            {
                var principal = _jwtService.ValidateToken(token);
                if (principal == null) return false;

                var username = principal.Identity?.Name;
                if (string.IsNullOrEmpty(username)) return false;

                using var scope = _scopeFactory.CreateScope();
                var customUserService = scope.ServiceProvider.GetRequiredService<CustomUserService>();

                var identity = await customUserService.BuildClaimsIdentityAsync(username);
                if (identity == null) return false;

                context.User = new ClaimsPrincipal(identity);
                
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}