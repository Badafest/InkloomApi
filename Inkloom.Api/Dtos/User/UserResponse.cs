namespace Inkloom.Api.Dtos;
public class UserResponse
{
    public bool EmailVerified { get; set; } = false;
    public bool ProfileComplete { get; set; } = false;
    public string? Avatar { get; set; }
    public string? About { get; set; }
    public AuthType[] AuthTypes { get; set; } = [];
    public string Email { get; set; } = "";
    public string Username { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public int Followers { get; set; } = 0;
    public int Following { get; set; } = 0;
    public int Blogs { get; set; } = 0;
}
