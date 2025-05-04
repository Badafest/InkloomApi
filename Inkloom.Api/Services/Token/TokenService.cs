
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;
using System.Text.Json.Nodes;

namespace Inkloom.Api.Services;

public partial class TokenService(IConfiguration config, ILogger<TokenService> logger) : ITokenService
{
    private readonly IConfiguration _config = config;
    private readonly ILogger<TokenService> _logger = logger;
    private readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

    private readonly string OTPValidCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    public string GenerateOTP(short length = 6, string? validCharacters = null)
    {
        var randomBytes = new byte[length];
        _rng.GetNonZeroBytes(randomBytes);
        var code = "";
        var characters = validCharacters ?? OTPValidCharacters;
        foreach (var randomByte in randomBytes)
        {
            code += characters[randomByte % (characters.Length - 1)];
        }
        return code;
    }
    public string GenerateJWT(string sub, string uniqueName, DateTime expiry, Dictionary<string, string>? otherClaims = null)
    {
        var signingKey = _config["Jwt:Key"] ?? "";
        var issuer = _config["Jwt:Issuer"] ?? "";
        var audience = _config["Jwt:Audience"] ?? "";

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var defaultClaims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, sub),
                new Claim(JwtRegisteredClaimNames.UniqueName, uniqueName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

        var claims = defaultClaims.Concat(otherClaims?.Select(entry => new Claim(entry.Key, entry.Value)) ?? []);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiry,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public ClaimsPrincipal ValidateInkloomToken(string token, bool checkExpiry = true)
    {
        try
        {
            var signingKey = _config["Jwt:Key"] ?? "";
            var issuer = _config["Jwt:Issuer"] ?? "";
            var audience = _config["Jwt:Audience"] ?? "";

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = checkExpiry,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new Exception();
            return principal;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Inkloom Token Exception");
            throw new SecurityTokenException("Invalid Token");
        }
    }

    public async Task<User> ValidateGoogleToken(string token)
    {
        try
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync($"https://www.googleapis.com/oauth2/v1/userinfo?access_token={token}") ?? throw new Exception();
            var payload = JsonNode.Parse(response) ?? throw new Exception();
            var email = payload["email"]?.GetValue<string>() ?? throw new Exception();

            if (!payload["verified_email"]?.GetValue<bool>() ?? throw new Exception())
            {
                throw new Exception();
            }
            var name = payload["name"]?.GetValue<string>() ?? "";
            return new()
            {
                Email = email,
                Username = DefaultUsername(name),
                DisplayName = name,
                Avatar = payload["picture"]?.GetValue<string>(),
                EmailVerified = true
            };
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Google Token Exception");
            throw new SecurityTokenException("Invalid Token");
        }
    }

    public async Task<User> ValidateFacebookToken(string token)
    {
        try
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync($"https://graph.facebook.com/me?fields=id,name,email,picture&access_token={token}") ?? throw new Exception();
            var payload = JsonNode.Parse(response) ?? throw new Exception();
            var facebookId = payload["id"]?.GetValue<string>() ?? throw new Exception();
            var name = payload["name"]?.GetValue<string>() ?? "";
            return new()
            {
                Email = payload["email"]?.GetValue<string>() ?? $"{facebookId}@facebook.local",
                Username = DefaultUsername(name),
                DisplayName = name,
                Avatar = payload["picture"]?["data"]?["url"]?.GetValue<string>(),
                FacebookId = facebookId,
            };
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Facebook Token Exception");
            throw new SecurityTokenException("Invalid Token");
        }
    }

    private string DefaultUsername(string name)
    {
        var testUsername = name.Split(" ").ElementAt(0).ToLower();
        var suffix = GenerateOTP(4, "0123456789");
        return (DefaultUsernameRegex().IsMatch(testUsername + suffix) ? testUsername : "user") + suffix;
    }


    [GeneratedRegex("^[a-z]+[0-9]{4}$")]
    public static partial Regex DefaultUsernameRegex();
}