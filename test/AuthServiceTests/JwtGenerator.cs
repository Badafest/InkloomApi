using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InkloomApi.Models;
using Microsoft.IdentityModel.Tokens;

namespace test;

public static class JwtGenerator
{
    public static string Generate(User user, DateTime expiry)
    {
        var signingKey = Configuration.config["Jwt:Key"] ?? string.Empty;
        var issuer = Configuration.config["Jwt:Issuer"] ?? string.Empty;
        var audience = Configuration.config["Jwt:Audience"] ?? string.Empty;

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiry,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}