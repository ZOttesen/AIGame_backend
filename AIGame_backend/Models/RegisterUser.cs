namespace AIGame_backend.Models;

public class RegisterUser(string username, string firstName, string lastName, string email, string password)
{
    public string Username { get; } = username;

    public string FirstName { get; } = firstName;

    public string LastName { get; } = lastName;

    public string Email { get; } = email;

    public string Password { get; } = password;
}
