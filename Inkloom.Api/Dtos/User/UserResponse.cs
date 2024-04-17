namespace Inkloom.Api.Dtos;
public class UserResponse
{
    public string Email { get; set; } = "";
    public string Username { get; set; } = "";

    public bool EmailVerified { get; set; } = false;

    public string? Avatar { get; set; }

    public string? About { get; set; }
}
