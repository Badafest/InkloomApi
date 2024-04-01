namespace InkloomApi.Dtos;
public class LoginRequest
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}

public class MagicLoginRequest
{
    public string Email { get; set; } = "";
    public string Token { get; set; } = "";
}