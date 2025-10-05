using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class JwtService
{
    private readonly IConfiguration _config;
    private readonly string? _secret;
    private readonly int _expirationMinutes;

    public JwtService(IConfiguration config)
    {
        _config = config;
        _secret = _config["Jwt:Secret"];
        var expirationValue = _config["Jwt:ExpirationMinutes"];
        if (expirationValue == null)
            throw new InvalidOperationException("JWT expiration minutes is not configured.");
        _expirationMinutes = int.Parse(expirationValue);
    }

    public string GenerateToken(ClaimsIdentity identity)
    {
        var handler = new JwtSecurityTokenHandler();
        if (string.IsNullOrEmpty(_secret))
            throw new InvalidOperationException("JWT secret is not configured.");
        var key = Encoding.UTF8.GetBytes(_secret);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: identity.Claims,
            expires: DateTime.UtcNow.AddMinutes(_expirationMinutes),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256)
        );

        return handler.WriteToken(token);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        if (string.IsNullOrEmpty(_secret))
            throw new InvalidOperationException("JWT secret is not configured.");
        var key = Encoding.UTF8.GetBytes(_secret);

        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _config["Jwt:Issuer"],
            ValidAudience = _config["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            return handler.ValidateToken(token, parameters, out _);
        }
        catch
        {
            return null;
        }
    }
}
