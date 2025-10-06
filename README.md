```bash
ec-project-api/
├── ec-project-api.sln        # File solution mở bằng Visual Studio
├── ec-project-api/           # Source code chính
│ ├── Controllers/            # Chứa API Controllers
│ ├── Data/                   # DbContext và cấu hình database
│ ├── DTO/                    # Data Transfer Objects
│ ├── Helper/                 # Các helper, utilities
│ ├── Interfaces/             # Interface cho Repository/Service
│ ├── Migrations/             # Các file migration của EF Core
│ ├── Models/                 # Các entity/model
│ ├── Repository/             # Repository pattern implement
│ ├── Program.cs              # Entry point
│ └── ec-project-api.csproj   # Project file
└── README.md
```

# mã hóa refreshToken bằng thuật toán rồi trả về cho người dùng lưu ở cookies

# muc đích sử dụng middleware
# để check token ở giữa ta dùng
[HttpGet]
[Authorize]
#
``` bash
// Sau khi middleware gán context.User, bạn có thể dùng trực tiếp trong controller:

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    [HttpGet("profile")]
    [Authorize] // Yêu cầu phải có token
    public IActionResult GetProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // ID user
        var username = User.Identity?.Name; // username
        var roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);

        return Ok(new {
            UserId = userId,
            Username = username,
            Roles = roles
        });
    }

    [HttpGet("admin")]
    [Authorize(Roles = "Admin")] // Chỉ Admin mới vào được
    public IActionResult GetAdminData()
    {
        return Ok("Chỉ admin mới thấy được dữ liệu này");
    }
}
```