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
            Email = request.Email,
            Password = hashedPassword,
            FirstName = request.FirstName,
            LastName = request.LastName,
        };

        context.Users.Add(user);
        await context.SaveChangesAsync(); // Save changes in the DB context to generate the id for the user

        return Ok(user);
    }
}
