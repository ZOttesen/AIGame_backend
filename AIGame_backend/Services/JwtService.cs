namespace AIGame_backend.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

public class JwtService
{
    private readonly string _jwtSecret;

    /// <summary>
    /// Initializes a new instance of the <see cref="JwtService"/> class.
    /// </summary>
    public JwtService()
    {
        _jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
                     ?? throw new ArgumentNullException(nameof(_jwtSecret), "Environment variable JWT_SECRET is not set.");
    }

    /// <summary>
    /// Generates a JWT token for the user with the given id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public string GenerateToken(Guid id)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, id.ToString()),
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Convert.FromBase64String(_jwtSecret)),
                SecurityAlgorithms.HmacSha256Signature),
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public ClaimsPrincipal ValidateToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(_jwtSecret)),
            ValidateIssuer = false,
            ValidateAudience = false,
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
            return principal;
        }
        catch (SecurityTokenException ex)
        {
            throw new UnauthorizedAccessException("Invalid token", ex);
        }
    }

    public Guid ExtractGuid(ClaimsPrincipal principal)
    {
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userGuid))
        {
            throw new UnauthorizedAccessException("Invalid token or no user ID found");
        }

        return userGuid;
    }

}