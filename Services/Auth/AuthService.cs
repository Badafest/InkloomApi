using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace InkloomApi.Services
{
    public class AuthService : IAuthService
    {

        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public enum TokenType { Access, Refresh };

        public AuthService(IConfiguration config, IMapper mapper)
        {
            _config = config;
            _mapper = mapper;
        }
        async public Task<ServiceResponse<LoginResponse>> Login(LoginRequest credentials)
        {
            var user = UserData.FindUser(credentials.Username);
            if (user == null || user.Password != credentials.Password)
            {
                return new(HttpStatusCode.BadRequest) { Message = "Incorrect Username or Password" };
            }
            return new() { Data = UpdateUserTokens(user) };
        }

        async public Task<ServiceResponse<LoginResponse>> Refresh(RefreshRequest credentials)
        {
            var principalAccess = ValidatJwt(credentials.AccessToken, false);
            var principalRefresh = ValidatJwt(credentials.RefreshToken);
            var username = principalAccess?.Identity?.Name ?? string.Empty;
            var user = UserData.FindUser(username);
            if (user == null || principalRefresh?.Identity?.Name != user.Username || credentials.RefreshToken != user.RefreshToken || user.RefreshTokenExpiry <= DateTime.Now)
            {
                return new(HttpStatusCode.BadRequest) { Message = "Invalid Tokens" };
            }
            return new() { Data = UpdateUserTokens(user) };
        }

        private LoginResponse UpdateUserTokens(User user)
        {
            var accessToken = GenerateJwt(user, TokenType.Access);
            var refreshToken = GenerateJwt(user, TokenType.Refresh);
            var refreshTokenExpiry = DateTime.Now.AddMinutes(int.Parse(_config["Jwt:Expiry:Refresh"] ?? "120"));
            UserData.UpdateUser(new() { Username = user.Username, RefreshToken = refreshToken, RefreshTokenExpiry = refreshTokenExpiry });
            return new() { Username = user.Username, AccessToken = accessToken, RefreshToken = refreshToken };
        }
        private string GenerateJwt(User user, TokenType type)
        {
            var signingKey = _config["Jwt:Key"] ?? string.Empty;
            var issuer = _config["Jwt:Issuer"] ?? string.Empty;
            var audience = _config["Jwt:Audience"] ?? string.Empty;
            var expiry = int.Parse(_config[$"Jwt:Expiry:{type}"] ?? "120");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, user.Name),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("type", type.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(expiry),
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

        public ServiceResponse<UserResponse> Me(string username)
        {
            var user = UserData.FindUser(username);
            if (user == null || user.Username != username)
            {
                return new(HttpStatusCode.NotFound) { Message = "User not Found" };
            }
            return new() { Data = _mapper.Map<UserResponse>(user) };
        }
    }
}