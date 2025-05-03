using System.Net;
using System.Text.RegularExpressions;
using Inkloom.Api.Email;
using Inkloom.Api.EmailTemplates;
using Microsoft.IdentityModel.Tokens;

namespace Inkloom.Api.Services;
public partial class AuthService(IConfiguration config, DataContext context, IMapper mapper, ITokenService tokenService, IEmailService emailService) : IAuthService
{

    private readonly IConfiguration _config = config;
    private readonly IMapper _mapper = mapper;
    private readonly DataContext _context = context;

    private readonly ITokenService _tokenService = tokenService;

    private readonly IEmailService _emailService = emailService;

    private readonly SecurityTokenException _tokenException = new("Invalid Token");

    async public Task<ServiceResponse<UserResponse>> Register(RegisterRequest userData)
    {
        var newUser = new User
        {
            Username = userData.Type == AuthType.PASSWORD ? userData.Username : "user0000",
            DisplayName = userData.DisplayName,
            Password = userData.Type == AuthType.PASSWORD ? userData.Password : "",
            Email = userData.Type == AuthType.PASSWORD ? userData.Email : "user@inkloom.local",
            TokenBlacklistTimestamp = DateTime.UtcNow,
            ProfileComplete = true
        };

        if (userData.Type != AuthType.PASSWORD)
        {
            var payload = userData.Type switch
            {
                AuthType.GOOGLE => await _tokenService.ValidateGoogleToken(userData.Password),
                AuthType.FACEBOOK => await _tokenService.ValidateFacebookToken(userData.Password),
                _ => throw new ArgumentException("Invalid auth type")
            };

            // replace user properties
            newUser.Avatar = payload.Avatar;
            newUser.FacebookId = payload.FacebookId;
            newUser.Email = payload.Email;
            newUser.Username = payload.Username;
            newUser.DisplayName = payload.DisplayName;
            newUser.EmailVerified = payload.EmailVerified;
            newUser.ProfileComplete = !MissingUsernameRegex().IsMatch(payload.Username) && !payload.Email.EndsWith(".local");
            newUser.AuthTypes = [userData.Type];
        }

        var oldUser = await _context.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(user => user.Username == newUser.Username || user.Email == newUser.Email);

        if (oldUser?.Username == newUser.Username)
        {
            return new(HttpStatusCode.BadRequest) { Message = "Username is already Taken" };
        }
        if (oldUser?.Email == newUser.Email)
        {
            return new(HttpStatusCode.BadRequest) { Message = "Email is already Taken" };
        }

        _context.Users.Add(newUser);
        await _context.SoftSaveChangesAsync();
        return new() { Data = _mapper.Map<UserResponse>(newUser) };
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

        var principalRefresh = _tokenService.ValidateInkloomToken(credentials.RefreshToken);
        var principalAccess = _tokenService.ValidateInkloomToken(credentials.AccessToken, false);
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
        var principal = _tokenService.ValidateInkloomToken(tokenValue, true);
        var username = principal?.Identity?.Name ?? "";
        var user = await _context.Users.FirstOrDefaultAsync(user => user.Username == username);
        if (user?.Username != username)
        {
            throw _tokenException;
        }

        var verifyToken = await _context.Tokens.FirstOrDefaultAsync(
            token => token.Type == TokenType.MagicLink &&
                token.Value == tokenValue &&
                token.UserId == user.Id) ?? throw _tokenException;

        _context.Tokens.Remove(verifyToken);

        if (verifyToken.Value != tokenValue || verifyToken.Expiry < DateTime.UtcNow)
        {

            await _context.SaveChangesAsync();
            throw _tokenException;
        }

        user.AuthTypes = [.. user.AuthTypes, AuthType.MAGICLINK];

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
        }.GetValueOrDefault(tokenType), _config["WebBaseUrl"]);

        await _emailService.SendEmail(new()
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

        var template = new MagicLoginTemplate(user.Username, _config["WebBaseUrl"], magicToken);
        await _emailService.SendEmail(new()
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

    public async Task<ServiceResponse<LoginResponse?>> SsoLogin(string tokenValue, AuthType authType)
    {
        var payload = authType switch
        {
            AuthType.GOOGLE => await _tokenService.ValidateGoogleToken(tokenValue),
            AuthType.FACEBOOK => await _tokenService.ValidateFacebookToken(tokenValue),
            _ => throw new ArgumentException("Invalid auth type")
        };
        // find user by email or facebook id to sync multiple sso providers
        var user = await _context.Users.FirstOrDefaultAsync(
            user => !string.IsNullOrEmpty(user.Email) && user.Email == payload.Email ||
            !string.IsNullOrEmpty(user.FacebookId) && user.FacebookId == payload.FacebookId);

        if (user != null)
        {
            // update user profile info
            user.Avatar ??= payload.Avatar;
            user.FacebookId ??= payload.FacebookId;
            user.Email ??= payload.Email;
            user.DisplayName ??= payload.DisplayName;
            user.EmailVerified = user.EmailVerified || payload.EmailVerified;
            user.ProfileComplete = !MissingUsernameRegex().IsMatch(user.Username) && !user.Email.EndsWith(".local");
            user.AuthTypes = [.. user.AuthTypes, authType];
            await _context.SaveChangesAsync();
            return new() { Data = await GenerateAuthTokens(user) };
        }

        var newUser = new User
        {
            Avatar = payload.Avatar,
            FacebookId = payload.FacebookId,
            Email = payload.Email,
            DisplayName = payload.DisplayName,
            EmailVerified = payload.EmailVerified,
            ProfileComplete = !MissingUsernameRegex().IsMatch(payload.Username) && !payload.Email.EndsWith(".local"),
            Username = payload.Username,
            Password = "",
            TokenBlacklistTimestamp = DateTime.UtcNow,
            AuthTypes = [authType]
        };

        _context.Users.Add(newUser);
        await _context.SoftSaveChangesAsync();

        return new() { Data = await GenerateAuthTokens(newUser) };
    }

    [GeneratedRegex("user\\d{4}")]
    private static partial Regex MissingUsernameRegex();
}