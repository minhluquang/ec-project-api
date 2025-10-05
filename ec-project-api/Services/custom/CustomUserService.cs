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
        if (user == null)
            throw new KeyNotFoundException(UserMessages.UserNotFound);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email)
        };

        foreach (var role in user.UserRoleDetails.Select(r => r.Role))
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Name));
        }

        return new ClaimsIdentity(claims, "Custom");
    }
}
