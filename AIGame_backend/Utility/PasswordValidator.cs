namespace AIGame_backend.Utility;

using System.Text.RegularExpressions;

public class PasswordValidator
{
    public void Validate(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password cannot be empty or whitespace.");
        }

        if (password.Length < 8)
        {
            throw new ArgumentException("Password must be at least 8 characters long.");
        }

        if (!Regex.IsMatch(password, @"[a-z]"))
        {
            throw new ArgumentException("Password must contain at least one lowercase letter.");
        }

        if (!Regex.IsMatch(password, @"[A-Z]"))
        {
            throw new ArgumentException("Password must contain at least one uppercase letter.");
        }

        if (!Regex.IsMatch(password, @"[0-9]"))
        {
            throw new ArgumentException("Password must contain at least one digit.");
        }
    }
}