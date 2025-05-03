namespace Inkloom.Api.Dtos;
public class RegisterRequest
{
    public AuthType Type { get; set; } = AuthType.PASSWORD;
    public string Email { get; set; } = "";
    public string Username { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string Password { get; set; } = "";
}