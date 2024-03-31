namespace InkloomApi.Services;

public interface IAuthService
{
    Task<ServiceResponse<UserResponse>> Register(RegisterRequest userData);
    Task<ServiceResponse<LoginResponse>> Login(LoginRequest credentials);
    Task<ServiceResponse<LoginResponse>> MagicLogin(string tokenValue);
    Task<ServiceResponse<LoginResponse>> Refresh(RefreshRequest credentials);
    Task<ServiceResponse<UserResponse?>> GenerateAndSendOTP(string email, TokenType tokenType);

    Task<User?> VerifyOTP(string tokenValue, TokenType tokenType, string email);
}
