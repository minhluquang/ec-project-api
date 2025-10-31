using System.Text.Json;

namespace ec_project_api.Security
{
    public class CustomAuthenticationEntryPoint
    {
        public async Task HandleAsync(HttpContext context)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var response = new
            {
                success = false,
                message = "Không được phép: Vui lòng đăng nhập để truy cập tài nguyên này."
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
