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

        return Ok("User created successfully");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUser request)
    {
        var user = await _userService.FirstOrDefaultAsync(request.Email);
        _hashing.Verify(request.Password, user.Password);
        var token = _jwtService.GenerateToken(user.UserGuid, user.Username);

        return Ok(new { token });
    }

    [HttpPost("edit-user")]
    public async Task<IActionResult> EditUser([FromBody] EditUserRequest request)
    {
        ClaimsPrincipal token = _jwtService.ValidateToken(request.Token);
        Guid userGuid = _jwtService.ExtractGuid(token);
        User user = await _userService.FirstOrDefaultAsync(userGuid);

        await _userService.UpdateUser(user, request);
        return Ok("User updated successfully");
    }

    [HttpPost("delete-user")]
    public async Task<IActionResult> DeleteUser([FromBody] DeleteUserRequest request)
    {
        ClaimsPrincipal token = _jwtService.ValidateToken(request.Token);
        Guid userGuid = _jwtService.ExtractGuid(token);
        User user = await _userService.FindUserByGuidAsync(userGuid);

        _hashing.Verify(request.Password, user.Password);
        await _userService.Remove(user);

        return Ok("User deleted successfully");
    }

    [HttpPost("change-password")]
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
