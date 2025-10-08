using ec_project_api.Constants.messages;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class JwtService
{
    private readonly IConfiguration _config;
    private readonly string _secret;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _accessTokenMinutes;
    private readonly int _refreshTokenDays;
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public JwtService(IConfiguration config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _secret = _config["Jwt:Secret"] ?? throw new InvalidOperationException(JwtMessages.JwtSecretNotConfigured);
        _issuer = _config["Jwt:Issuer"] ?? throw new InvalidOperationException(JwtMessages.JwtIssuerNotConfigured);
        _audience = _config["Jwt:Audience"] ?? throw new InvalidOperationException(JwtMessages.JwtAudienceNotConfigured);
        _tokenHandler = new JwtSecurityTokenHandler();

        if (!int.TryParse(_config["Jwt:ExpirationMinutes"], out _accessTokenMinutes))
            throw new InvalidOperationException(JwtMessages.JwtExpirationNotConfigured);

        if (!int.TryParse(_config["Jwt:RefreshExpirationDays"], out _refreshTokenDays))
            throw new InvalidOperationException(JwtMessages.JwtRefreshExpirationNotConfigured);
    }

    public string GenerateToken(ClaimsIdentity identity) =>
        GenerateJwt(identity.Claims, DateTime.UtcNow.AddMinutes(_accessTokenMinutes));

    public string GenerateRefreshToken(ClaimsIdentity identity)
    {
        var claims = new List<Claim>(identity.Claims)
        {
            new Claim("rtid", Guid.NewGuid().ToString())
        };
        return GenerateJwt(claims, DateTime.UtcNow.AddDays(_refreshTokenDays));
    }

    public string GenerateEmailVerificationToken(string email)
    {
        var claims = new[] { new Claim(ClaimTypes.Email, email) };
        return GenerateJwt(claims, DateTime.UtcNow.AddMinutes(5));
    }

    private string GenerateJwt(IEnumerable<Claim> claims, DateTime expires)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials
        );

        return _tokenHandler.WriteToken(token);
    }

    public ClaimsPrincipal ValidateToken(string token)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _issuer,
            ValidAudience = _audience,
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            return _tokenHandler.ValidateToken(token, parameters, out _);
        }
        catch (SecurityTokenExpiredException)
        {
            throw new UnauthorizedAccessException(JwtMessages.TokenExpired);
        }
        catch (SecurityTokenInvalidSignatureException)
        {
            throw new UnauthorizedAccessException(JwtMessages.InvalidSignature);
        }
        catch (Exception)
        {
            throw new UnauthorizedAccessException(JwtMessages.InvalidToken);
        }
    }

    public DateTime GetRefreshTokenExpiryDate() => DateTime.UtcNow.AddDays(_refreshTokenDays);
}
