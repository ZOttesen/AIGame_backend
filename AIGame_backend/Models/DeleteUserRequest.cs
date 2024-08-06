namespace AIGame_backend.Models;

public class DeleteUserRequest(string token, string password)
{
    public string Token { get; } = token;

    public string Password { get; } = password;
}