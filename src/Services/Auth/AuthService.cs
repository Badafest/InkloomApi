using System.Net;
using Microsoft.IdentityModel.Tokens;

namespace InkloomApi.Services;
public class AuthService(IConfiguration config, DataContext context, IMapper mapper, ITokenService tokenService) : IAuthService
{

    private readonly IConfiguration _config = config;
    private readonly IMapper _mapper = mapper;
    private readonly DataContext _context = context;

    private readonly ITokenService _tokenService = tokenService;

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
        await _context.SoftSaveChangesAsync();
        return new() { Data = _mapper.Map<UserResponse>(user) };
    }

    async public Task<ServiceResponse<LoginResponse>> Login(LoginRequest credentials)
    {
        var user = await _context.Users.FirstOrDefaultAsync(user => user.Email == credentials.Email);
        if (user == null || !user.VerifyPassword(credentials.Password))
        {
            return new(HttpStatusCode.BadRequest) { Message = "Incorrect Email or Password" };
        }
        return new() { Data = await GenerateAuthTokens(user) };
    }

    async public Task<ServiceResponse<LoginResponse>> Refresh(RefreshRequest credentials)
    {

        var principalRefresh = _tokenService.ValidateJWT(credentials.RefreshToken);
        var principalAccess = _tokenService.ValidateJWT(credentials.AccessToken, false);
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

        var extraClaims = new Dictionary<string, string>([
            new("email_verified", user.EmailVerified ? "true" : "false")
        ]);

        var accessToken = _tokenService.GenerateJWT(user.Email, user.Username, accessTokenExpiry, extraClaims);

        var refreshTokenExpiry = DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:Expiry:Refresh"] ?? "4320"));
        var refreshToken = _tokenService.GenerateJWT(user.Email, user.Username, refreshTokenExpiry);

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
}