namespace Inkloom.Api.Dtos;
public class TokenResponse
{
    public string Value { get; set; } = "";
    public DateTime Expiry { get; set; }
}
public class LoginResponse
{
    public string Username { get; set; } = "";
    public TokenResponse AccessToken { get; set; } = new();

    public TokenResponse RefreshToken { get; set; } = new();

}
