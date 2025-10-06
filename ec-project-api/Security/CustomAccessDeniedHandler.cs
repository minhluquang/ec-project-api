using System.Text.Json;

namespace ec_project_api.Security
{
    public class CustomAccessDeniedHandler
    {
        public async Task HandleAsync(HttpContext context)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            var response = new
            {
                success = false,
                message = "Truy cập bị từ chối: Bạn không có quyền."
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
