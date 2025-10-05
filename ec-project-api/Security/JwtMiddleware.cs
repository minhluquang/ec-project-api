using System.Security.Claims;

namespace ec_project_api.Security
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtService _jwtService;
        private readonly IServiceProvider _serviceProvider;

        public JwtMiddleware(RequestDelegate next, JwtService jwtService, IServiceProvider serviceProvider)
        {
            _next = next;
            _jwtService = jwtService;
            _serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token))
            {
                await AttachUserToContext(context, token);
            }

            await _next(context);
        }

        private async Task AttachUserToContext(HttpContext context, string token)
        {
            var principal = _jwtService.ValidateToken(token);
            if (principal == null) return;

            var username = principal.Identity?.Name;
            if (string.IsNullOrEmpty(username)) return;

            // Tạo scope để resolve CustomUserService
            using var scope = _serviceProvider.CreateScope();
            var customUserService = scope.ServiceProvider.GetRequiredService<CustomUserService>();

            var identity = await customUserService.BuildClaimsIdentityAsync(username);
            context.User = new ClaimsPrincipal(identity);
        }
    }
}
