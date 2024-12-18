namespace AIGame_backend.Controllers;

using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services;
using Utility;

[ApiController]
[Route("v1")]
public class AuthApiController : ControllerBase
{
    private readonly Hashing _hashing;
    private readonly PasswordValidator _passwordValidator;
    private readonly JwtService _jwtService;
    private readonly UserService _userService;

    public AuthApiController(Hashing hashing, PasswordValidator passwordValidator, JwtService jwtService, UserService userService)
    {
        _hashing = hashing;
        _passwordValidator = passwordValidator;
        _jwtService = jwtService;
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUser request)
    {
        try
        {
            _passwordValidator.Validate(request.Password);

            var hashedPassword = _hashing.Hash(request.Password);

            await _userService.EmailExists(request.Email);
            await _userService.UserExists(request.Username);

            var user = new User
            {
                Username = request.Username,
                Email = request.Email.ToLower(),
                Password = hashedPassword,
                FirstName = request.FirstName,
                LastName = request.LastName,
            };

            await _userService.Add(user);

            return Ok(new { message = "User created successfully" });
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An unexpected error occurred", details = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUser request)
    {
        try
        {
            var user = await _userService.FirstOrDefaultAsync(request.Email);

            _hashing.Verify(request.Password, user.Password);

            var token = _jwtService.GenerateToken(user.UserGuid);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(1),
            };
            Response.Cookies.Append("authToken", token, cookieOptions);

            Console.WriteLine($"Cookie options: {cookieOptions}");

            return Ok(new { message = "Login successful" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during login: {ex.Message}");
            return StatusCode(500, new { message = "An error occurred during login" });
        }
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/",
            Expires = DateTime.UtcNow.AddDays(-1),
        };

        Response.Cookies.Append("authToken", string.Empty, cookieOptions);

        return Ok(new { message = "Logged out successfully" });
    }

    [HttpGet("user")]
    public async Task<IActionResult> GetUser()
    {
        var userClaims = HttpContext.Items["User"] as ClaimsPrincipal;

        if (userClaims == null)
        {
            return Unauthorized(new { message = "Unauthorized" });
        }

        try
        {
            var userGuid = Guid.Parse(userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
            var user = await _userService.FirstOrDefaultAsync(userGuid);

            return Ok(new
            {
                userGuid = user.UserGuid,
                firstName = user.FirstName,
                lastName = user.LastName,
                email = user.Email,
                username = user.Username,
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving user: {ex.Message}");
            return StatusCode(500, new { message = "An error occurred while retrieving user data" });
        }
    }

    [HttpPatch("user")]
    public async Task<IActionResult> EditUser([FromBody] EditUserRequest request)
    {
        var user = HttpContext.Items["User"] as User;
        if (user == null)
        {
            return Unauthorized(new { message = "Unauthorized" });
        }

        await _userService.UpdateUser(user, request);
        return Ok("User updated successfully");
    }


    [HttpDelete("user")]
    public async Task<IActionResult> DeleteUser([FromBody] DeleteUserRequest request)
    {
        ClaimsPrincipal token = _jwtService.ValidateToken(request.Token);
        Guid userGuid = _jwtService.ExtractGuid(token);
        User user = await _userService.FindUserByGuidAsync(userGuid);

        _hashing.Verify(request.Password, user.Password);
        await _userService.Remove(user);

        return Ok("User deleted successfully");
    }

    [HttpPatch("password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        ClaimsPrincipal token = _jwtService.ValidateToken(request.Token);
        Guid userGuid = _jwtService.ExtractGuid(token);
        User user = await _userService.FirstOrDefaultAsync(userGuid);

        _hashing.Verify(request.OldPassword, user.Password);
        _passwordValidator.Validate(request.NewPassword);

        user.Password = _hashing.Hash(request.NewPassword);
        await _userService.SaveChangesAsync();

        return Ok("Password changed successfully");
    }
}
