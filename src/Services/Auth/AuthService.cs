using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace InkloomApi.Services
{
    public class AuthService(IConfiguration config, DataContext context, IMapper mapper) : IAuthService
    {

        private readonly IConfiguration _config = config;
        private readonly IMapper _mapper = mapper;
        private readonly DataContext _context = context;

        async public Task<ServiceResponse<UserResponse>> Register(RegisterRequest userData)
        {
            var oldUser = await _context.Users.FirstOrDefaultAsync(user => user.Username == userData.Username || user.Email == userData.Email);
            if (oldUser?.Username == userData.Username)
            {
                return new(HttpStatusCode.BadRequest) { Message = "Username is already Taken" };
            }
            if (oldUser?.Email == userData.Email)
            {
                return new(HttpStatusCode.BadRequest) { Message = "Email is already Taken" };
            }
            var user = new User { Username = userData.Username, Password = userData.Password, Email = userData.Email };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new() { Data = _mapper.Map<UserResponse>(user) };
        }

        async public Task<ServiceResponse<LoginResponse>> Login(LoginRequest credentials)
        {
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Username == credentials.Username);
            if (user == null || !user.VerifyPassword(credentials.Password))
            {
                return new(HttpStatusCode.BadRequest) { Message = "Incorrect Username or Password" };
            }
            return new() { Data = await GenerateAuthTokens(user) };
        }

        async public Task<ServiceResponse<LoginResponse>> Refresh(RefreshRequest credentials)
        {

            var principalRefresh = ValidateJwt(credentials.RefreshToken);
            var principalAccess = ValidateJwt(credentials.AccessToken, false);
            var username = principalAccess?.Identity?.Name ?? string.Empty;
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Username == username);

            if (user == null || principalRefresh?.Identity?.Name != user.Username)
            {
                throw new SecurityTokenException("Invalid Token");
            }
            var refreshToken = await _context.Tokens.FirstOrDefaultAsync(token => token.Type == TokenType.RefreshToken && token.UserId == user.Id && token.Value == credentials.RefreshToken) ?? throw new SecurityTokenException("Invalid Tokens");
            if (refreshToken.Value != credentials.RefreshToken || refreshToken.Expiry < DateTime.UtcNow)
            {

                _context.Tokens.Remove(refreshToken);
                await _context.SaveChangesAsync();
                throw new SecurityTokenException("Invalid Token");
            }
            return new() { Data = await GenerateAuthTokens(user, refreshToken) };
        }

        private async Task<LoginResponse> GenerateAuthTokens(User user, Token? oldRefreshToken = null)
        {
            var accessTokenExpiry = DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:Expiry:Access"] ?? "120"));
            var accessToken = GenerateJwt(user, accessTokenExpiry);

            var refreshTokenExpiry = DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:Expiry:Refresh"] ?? "4320"));
            var refreshToken = GenerateJwt(user, refreshTokenExpiry);

            if (oldRefreshToken != null)
            {
                oldRefreshToken.Value = refreshToken;
                oldRefreshToken.Expiry = refreshTokenExpiry;
                oldRefreshToken.Type = TokenType.RefreshToken;
                oldRefreshToken.UserId = user.Id;
            }
            else
            {
                _context.Tokens.Add(new() { Value = refreshToken, Expiry = refreshTokenExpiry, Type = TokenType.RefreshToken, UserId = user.Id });
            }
            await _context.SaveChangesAsync();

            return new()
            {
                Username = user.Username,
                AccessToken = new() { Value = accessToken, Expiry = accessTokenExpiry },
                RefreshToken = new() { Value = refreshToken, Expiry = refreshTokenExpiry }
            };
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
                notBefore: DateTime.UtcNow,
                expires: expiry,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private ClaimsPrincipal ValidateJwt(string token, bool checkExpiry = true)
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
}