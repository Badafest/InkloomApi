namespace InkloomApi.Models;
public enum TokenType { RefreshToken, EmailVerification, PasswordReset, MagicLink }
public class Token
{
    public int Id { get; set; } = 0;
    public int UserId { get; set; } = 0;
    public User? User { get; set; }

    public TokenType Type { get; set; }

    public string Value { get; set; } = "";

    public DateTime Expiry { get; set; }
}