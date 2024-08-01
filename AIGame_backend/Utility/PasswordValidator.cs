namespace AIGame_backend.Utility;

using System.Text.RegularExpressions;

public class PasswordValidator
{
    public bool Validate(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return false;
        }

        if (password.Length < 8)
        {
            return false;
        }

        if (!Regex.IsMatch(password, @"[a-z]"))
        {
            return false;
        }

        if (!Regex.IsMatch(password, @"[A-Z]"))
        {
            return false;
        }

        if (!Regex.IsMatch(password, @"[0-9]"))
        {
            return false;
        }

        return true;
    }
}