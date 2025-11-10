using ec_project_api.Constants.Messages;
using ec_project_api.Services;
using System.Security.Claims;

public class CustomUserService
{
    private readonly IUserService _userService;

    public CustomUserService(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<ClaimsIdentity> BuildClaimsIdentityAsync(string username)
    {
        var user = await _userService.FirstOrDefaultAsync(u => u.Username == username);
        var roles = user.UserRoleDetails.Select(ur => ur.Role).ToList();
        var permissions = roles.SelectMany(r => r.RolePermissions).Select(rp => rp.PermissionId).ToList();

        if (user == null)
            throw new KeyNotFoundException(UserMessages.UserNotFound);

        var claims = new List<Claim>
        {
            new Claim("UserId", user.UserId.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email)
        };

        foreach (var role in user.UserRoleDetails.Select(r => r.Role))
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Name));
        }

        foreach (var role in user.UserRoleDetails.Select(r => r.Role))
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Name));

            // Add permissions for this role
            foreach (var rp in role.RolePermissions)
            {
                claims.Add(new Claim("permission", rp.Permission.PermissionName));
            }
        }

        return new ClaimsIdentity(claims, "CustomUser");
    }
}
