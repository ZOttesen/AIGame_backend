namespace AIGame_backend.Models;

public partial class EditUserRequest
{
    public string Token { get; set; }

    public string? Username { get; set; }

    public string? Email { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }
}