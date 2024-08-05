namespace AIGame_backend.Utility;

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

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(new[] { new Claim("id", id.ToString()) }),
            Expires = DateTime.UtcNow.AddMinutes(3),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Convert.FromBase64String(_jwtSecret)),
                SecurityAlgorithms.HmacSha256Signature),
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public bool ValidateToken(string token)
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
            tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
        }
        catch (SecurityTokenException)
        {
            return false;
        }

        return true;
    }
}