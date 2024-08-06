namespace AIGame_backend.Models;

public class ChangePasswordRequest(string token, string oldPassword, string newPassword)
{
    public string Token { get; } = token;
    public string OldPassword { get; } = oldPassword;
    public string NewPassword { get; } = newPassword;
}