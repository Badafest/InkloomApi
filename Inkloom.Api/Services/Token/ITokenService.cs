namespace Inkloom.Api.Services;

public interface ITokenService
{
    string GenerateOTP(short length = 8, string? validCharacters = null);
    string GenerateJWT(string sub, string uniqueName, DateTime expiry, Dictionary<string, string>? otherClaims = null);
    ClaimsPrincipal ValidateInkloomToken(string token, bool checkExpiry = true);
    Task<User> ValidateGoogleToken(string token);
    Task<User> ValidateFacebookToken(string token);
}