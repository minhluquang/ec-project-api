using System.Security.Claims;

namespace ec_project_api.Security
{
    public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly JwtService _jwtService;
    private readonly CustomUserService _customUserService;

    public JwtMiddleware(RequestDelegate next, JwtService jwtService, CustomUserService customUserService)
    {
        _next = next;
        _jwtService = jwtService;
        _customUserService = customUserService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"]
            .FirstOrDefault()?.Split(" ").Last();

        if (token != null)
            await AttachUserToContext(context, token);

        await _next(context);
    }

    private async Task AttachUserToContext(HttpContext context, string token)
    {
        var principal = _jwtService.ValidateToken(token);
        if (principal == null) return;

        var username = principal.Identity?.Name;
        if (string.IsNullOrEmpty(username)) return;

        var identity = await _customUserService.BuildClaimsIdentityAsync(username);
        context.User = new ClaimsPrincipal(identity);
    }
}
}