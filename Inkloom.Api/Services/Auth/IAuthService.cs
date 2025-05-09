namespace Inkloom.Api.Services;

public interface IAuthService
{
    Task<ServiceResponse<UserResponse>> Register(RegisterRequest userData);
    Task<ServiceResponse<LoginResponse>> Login(LoginRequest credentials);
    Task<ServiceResponse<LoginResponse?>> MagicLogin(string tokenValue);
    Task<ServiceResponse<LoginResponse?>> SsoLogin(string tokenValue, AuthType authType);
    Task<ServiceResponse<LoginResponse>> Refresh(RefreshRequest credentials);
    Task<ServiceResponse<UserResponse?>> GenerateAndSendOTP(string email, TokenType tokenType);
    Task<ServiceResponse<LoginResponse?>> GenerateAndSendMagicToken(string email);

    Task<User?> VerifyOTP(string tokenValue, TokenType tokenType, string email);
}
