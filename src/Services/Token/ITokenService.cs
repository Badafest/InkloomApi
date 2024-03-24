namespace InkloomApi.Services;

public interface ITokenService
{
    int GenerateOTP(short length = 6);
    string GenerateJWT(string sub, string uniqueName, DateTime expiry);

    ClaimsPrincipal ValidateJWT(string token, bool checkExpiry = true);
}