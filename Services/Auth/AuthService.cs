using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace InkloomApi.Services
{
    public class AuthService : IAuthService
    {

        private readonly IConfiguration _config;
        private readonly DataContext _context;

        public AuthService(IConfiguration config, DataContext context)
        {
            _config = config;
            _context = context;
        }
        async public Task<ServiceResponse<LoginResponse>> Login(LoginRequest credentials)
        {
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Username == credentials.Username);
            if (user == null || user.Password != credentials.Password)
            {
                return new(HttpStatusCode.BadRequest) { Message = "Incorrect Username or Password" };
            }
            return new() { Data = await UpdateUserTokens(user) };
        }

        async public Task<ServiceResponse<LoginResponse>> Refresh(RefreshRequest credentials)
        {
            var principalAccess = ValidatJwt(credentials.AccessToken, false);
            var principalRefresh = ValidatJwt(credentials.RefreshToken);
            var username = principalAccess?.Identity?.Name ?? string.Empty;
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Username == username);
            if (user == null || principalRefresh?.Identity?.Name != user.Username)
            {
                return new(HttpStatusCode.BadRequest) { Message = "Invalid Tokens" };
            }
            var refreshToken = await _context.Tokens.FirstOrDefaultAsync(token => token.Type == TokenType.RefreshToken && token.UserId == user.Id);
            if (refreshToken != null)
            {
                _context.Tokens.Remove(refreshToken);
                await _context.SaveChangesAsync();
            }
            if (refreshToken == null || refreshToken.Value != credentials.RefreshToken || refreshToken.Expiry < DateTime.Now)
            {
                return new(HttpStatusCode.BadRequest) { Message = "Invalid Tokens" };
            }
            return new() { Data = await UpdateUserTokens(user) };
        }

        private async Task<LoginResponse> UpdateUserTokens(User user)
        {
            var accessTokenExpiry = DateTime.Now.AddMinutes(int.Parse(_config["Jwt:Expiry:Access"] ?? "120"));
            var accessToken = GenerateJwt(user, accessTokenExpiry);

            var refreshTokenExpiry = DateTime.Now.AddMinutes(int.Parse(_config["Jwt:Expiry:Refresh"] ?? "4320"));
            var refreshToken = GenerateJwt(user, refreshTokenExpiry);

            await _context.Tokens.AddAsync(new() { Value = refreshToken, Type = TokenType.RefreshToken, Expiry = refreshTokenExpiry, UserId = user.Id });

            return new() { Username = user.Username, AccessToken = accessToken, RefreshToken = refreshToken };
        }
        private string GenerateJwt(User user, DateTime expiry)
        {
            var signingKey = _config["Jwt:Key"] ?? string.Empty;
            var issuer = _config["Jwt:Issuer"] ?? string.Empty;
            var audience = _config["Jwt:Audience"] ?? string.Empty;

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
                notBefore: DateTime.Now,
                expires: expiry,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private ClaimsPrincipal ValidatJwt(string token, bool checkExpiry = true)
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
                throw new SecurityTokenException("Invalid Token");
            return principal;
        }
    }
}