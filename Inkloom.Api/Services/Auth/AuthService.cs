using System.Net;
using Inkloom.Api.Email;
using Inkloom.Api.EmailTemplates;
using Microsoft.IdentityModel.Tokens;

namespace Inkloom.Api.Services;
public class AuthService(IConfiguration config, DataContext context, IMapper mapper, ITokenService tokenService, IEmailService emailService) : IAuthService
{

    private readonly IConfiguration _config = config;
    private readonly IMapper _mapper = mapper;
    private readonly DataContext _context = context;

    private readonly ITokenService _tokenService = tokenService;

    private readonly IEmailService _emailService = emailService;

    private readonly SecurityTokenException _tokenException = new("Invalid Token");

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
        var user = new User { Username = userData.Username, Password = userData.Password, Email = userData.Email, TokenBlacklistTimestamp = DateTime.UtcNow };
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
        var username = principalAccess?.Identity?.Name ?? "";
        var user = await _context.Users.FirstOrDefaultAsync(user => user.Username == username);

        if (user == null || principalRefresh?.Identity?.Name != user.Username)
        {
            throw _tokenException;
        }
        var refreshToken = await _context.Tokens.FirstOrDefaultAsync(token => token.Type == TokenType.RefreshToken && token.UserId == user.Id && token.Value == credentials.RefreshToken) ?? throw _tokenException;
        if (refreshToken.Value != credentials.RefreshToken || refreshToken.Expiry < DateTime.UtcNow)
        {
            _context.Tokens.Remove(refreshToken);
            await _context.SaveChangesAsync();
            throw _tokenException;
        }
        return new() { Data = await GenerateAuthTokens(user, refreshToken) };
    }

    async public Task<ServiceResponse<LoginResponse?>> MagicLogin(string tokenValue)
    {

        var principal = _tokenService.ValidateJWT(tokenValue, true);
        var username = principal?.Identity?.Name ?? "";
        var user = await _context.Users.FirstOrDefaultAsync(user => user.Username == username);
        if (user?.Username != username)
        {
            throw _tokenException;
        }

        var verifyToken = await _context.Tokens.FirstOrDefaultAsync(token => token.Type == TokenType.MagicLink && token.Value == tokenValue && token.UserId == user.Id) ?? throw _tokenException;
        _context.Tokens.Remove(verifyToken);

        if (verifyToken.Value != tokenValue || verifyToken.Expiry < DateTime.UtcNow)
        {

            await _context.SaveChangesAsync();
            throw _tokenException;
        }

        _ = user.AuthTypes.Append(AuthType.MAGICLINK);

        await _context.SaveChangesAsync();

        return new() { Data = await GenerateAuthTokens(user) };
    }

    async public Task<ServiceResponse<UserResponse?>> GenerateAndSendOTP(string email, TokenType tokenType)
    {
        var user = await _context.Users.FirstOrDefaultAsync(user => user.Email == email);

        if (user?.Email != email)
        {
            return new() { };
        }

        var newOTP = _tokenService.GenerateOTP();
        var tokenValidity = int.Parse(_config[$"OtpExpiry:${tokenType}"] ?? "10");
        var tokenExpiry = DateTime.UtcNow.AddMinutes(tokenValidity);

        var oldToken = await _context.Tokens.FirstOrDefaultAsync(token => token.Type == tokenType && token.UserId == user.Id);
        if (oldToken?.Expiry > DateTime.UtcNow.AddMinutes(0.1 * tokenValidity))
        {
            return new() { };
        }
        if (oldToken != null)
        {
            oldToken.Value = newOTP;
            oldToken.Expiry = tokenExpiry;
        }
        else
        {
            await _context.Tokens.AddAsync(new() { Value = newOTP, Expiry = tokenExpiry, Type = tokenType, UserId = user.Id });
        }

        await _context.SaveChangesAsync();

        var template = new OTPTemplate(user.Username, newOTP, new Dictionary<TokenType, string>(){
            {TokenType.EmailVerification, "Email Verification"},
            {TokenType.PasswordReset, "Password Reset"},
        }.GetValueOrDefault(tokenType));
        _emailService.SendEmail(new()
        {
            To = new(user.Username, email),
            TextBody = template.GetTextBody(),
            HtmlBody = await template.GetHtmlBody(),
            Subject = new Dictionary<TokenType, string>(){
                {TokenType.EmailVerification, "Verify your Email"},
                {TokenType.PasswordReset, "Reset your Password"},
            }.GetValueOrDefault(tokenType),
        });

        return new() { };
    }

    public async Task<User?> VerifyOTP(string tokenValue, TokenType tokenType, string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(user => user.Email == email);
        if (user?.Email != email)
        {
            return null;
        }
        var dbToken = await _context.Tokens.FirstOrDefaultAsync(token => token.Type == tokenType && token.UserId == user.Id && token.Value == tokenValue);
        if (dbToken?.Value != tokenValue)
        {
            return null;
        }
        _context.Tokens.Remove(dbToken);
        await _context.SaveChangesAsync();
        if (dbToken.Expiry < DateTime.UtcNow)
        {
            return null;
        }
        return user;
    }

    public async Task<ServiceResponse<LoginResponse?>> GenerateAndSendMagicToken(string email)
    {
        var magicTokenValidity = int.Parse(_config["Jwt:Expiry:MagicLink"] ?? "10");
        var magicLinkTokenExpiry = DateTime.UtcNow.AddMinutes(magicTokenValidity);

        var user = await _context.Users.FirstOrDefaultAsync(user => user.Email == email && user.EmailVerified);
        var response = new ServiceResponse<LoginResponse?>(HttpStatusCode.OK) { Message = "You should receive an email with a login link if you have registered and verified your account earlier." };

        if (user?.Email != email)
        {
            return response;
        }

        var magicToken = _tokenService.GenerateJWT(user.Email, user.Username, magicLinkTokenExpiry);

        var oldMagicToken = await _context.Tokens.FirstOrDefaultAsync(token => token.Type == TokenType.MagicLink && token.UserId == user.Id);

        var waitingTime = oldMagicToken?.Expiry - DateTime.UtcNow.AddMinutes(0.6 * magicTokenValidity);
        if (waitingTime >= TimeSpan.FromMinutes(1))
        {
            response.Message = $"You requested another link earlier. Please try again after {waitingTime?.Minutes} minutes.";
            return response;
        }

        if (oldMagicToken != null)
        {
            oldMagicToken.Value = magicToken;
            oldMagicToken.Expiry = magicLinkTokenExpiry;
        }
        else
        {
            _context.Tokens.Add(new() { Value = magicToken, Expiry = magicLinkTokenExpiry, Type = TokenType.MagicLink, UserId = user.Id });
        }
        await _context.SaveChangesAsync();

        var template = new MagicLoginTemplate(user.Username, magicToken);
        _emailService.SendEmail(new()
        {
            To = new(user.Username, email),
            TextBody = template.GetTextBody(),
            HtmlBody = await template.GetHtmlBody(),
            Subject = "Login to Inkloom"
        });

        return response;
    }
    private async Task<LoginResponse> GenerateAuthTokens(User user, Token? oldRefreshToken = null)
    {
        var accessTokenExpiry = DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:Expiry:Access"] ?? "120"));

        var extraClaims = new Dictionary<string, string>([
            new("email_verified", user.EmailVerified ? "true" : "false"),
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