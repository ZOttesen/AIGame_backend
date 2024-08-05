namespace AIGame_backend.Models;

public class LoginUser(string email, string password)
{
    public string Email { get; } = email;

    public string Password { get; } = password;
}