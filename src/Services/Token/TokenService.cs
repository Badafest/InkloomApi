
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace InkloomApi.Services;

public class TokenService(IConfiguration config) : ITokenService
{
    private readonly IConfiguration _config = config;
    private readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

    private readonly string OTPValidCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    public string GenerateOTP(short length = 6)
    {
        var randomBytes = new byte[length];
        _rng.GetNonZeroBytes(randomBytes);
        var code = "";
        foreach (var randomByte in randomBytes)
        {
            code += OTPValidCharacters[randomByte % (OTPValidCharacters.Length - 1)];
        }
        return code;
    }

    public string GenerateJWT(string sub, string uniqueName, DateTime expiry, Dictionary<string, string>? otherClaims = null)
    {
        var signingKey = _config["Jwt:Key"] ?? string.Empty;
        var issuer = _config["Jwt:Issuer"] ?? string.Empty;
        var audience = _config["Jwt:Audience"] ?? string.Empty;

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

    public ClaimsPrincipal ValidateJWT(string token, bool checkExpiry = true)
    {
        try
        {
            var signingKey = _config["Jwt:Key"] ?? string.Empty;
            var issuer = _config["Jwt:Issuer"] ?? string.Empty;
            var audience = _config["Jwt:Audience"] ?? string.Empty;

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
        catch
        {
            throw new SecurityTokenException("Invalid Token");
        }
    }
}