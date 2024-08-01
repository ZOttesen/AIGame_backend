namespace AIGame_backend.Models;

public class RegisterUser(string firstName, string lastName, string email, string password)
{
    public string FirstName { get; } = firstName;

    public string LastName { get; } = lastName;

    public string Email { get; } = email;

    public string Password { get; } = password;
}
