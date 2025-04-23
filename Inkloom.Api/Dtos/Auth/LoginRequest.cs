namespace Inkloom.Api.Dtos;
public class LoginRequest
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}

public class MagicLoginRequest
{
    public string? Email { get; set; } = "";
    public string? Token { get; set; } = "";
}

public class SsoLoginRequest
{
    public AuthType Type { get; set; } = AuthType.GOOGLE;
    public string Token { get; set; } = "";
}