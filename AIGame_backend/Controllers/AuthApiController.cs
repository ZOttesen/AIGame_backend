namespace AIGame_backend.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Utility;

[ApiController]
[Route("v1")]
public class AuthApiController(UserContext context) : ControllerBase
{
    private readonly Hashing _hashing = new();
    private readonly PasswordValidator _passwordValidator = new();
    private readonly JwtService _jwtService = new();

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUser request)
    {
        if (!_passwordValidator.Validate(request.Password))
        {
            return BadRequest("Password does not meet the requirements");
        }

        var hashedPassword = _hashing.Hash(request.Password); // Hash the password before storing

        var latestUser = await context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (latestUser != null)
        {
            return BadRequest("User already exists");
        }

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            Password = hashedPassword,
            FirstName = request.FirstName,
            LastName = request.LastName,
        };

        context.Users.Add(user);
        await context.SaveChangesAsync(); // Save changes in the DB context to generate the id for the user

        return Ok("User created successfully");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUser request)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
        {
            return NotFound();
        }

        if (!_hashing.Verify(request.Password, user.Password))
        {
            return Unauthorized();
        }

        var token = _jwtService.GenerateToken(user.UserGuid);

        return Ok(new { token });
    }

    [HttpPost("edit-user")]
    public async Task<IActionResult> EditUser([FromBody] EditUserRequest request)
    {
        if (!_jwtService.ValidateToken(request.Token))
        {
            return Unauthorized();
        }

        var user = await context.Users.FirstOrDefaultAsync(u => u.UserGuid == request.UserGuid);

        if (user == null)
        {
            return NotFound();
        }

        user.Username = request.Username ?? user.Username;
        user.Email = request.Email ?? user.Email;
        user.FirstName = request.FirstName ?? user.FirstName;
        user.LastName = request.LastName ?? user.LastName;

        await context.SaveChangesAsync();

        return Ok("User updated successfully");
    }
}
