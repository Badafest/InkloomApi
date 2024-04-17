namespace Inkloom.Api.Services;

public interface ITokenService
{
    string GenerateOTP(short length = 8);
    string GenerateJWT(string sub, string uniqueName, DateTime expiry, Dictionary<string, string>? otherClaims = null);
    ClaimsPrincipal ValidateJWT(string token, bool checkExpiry = true);
}