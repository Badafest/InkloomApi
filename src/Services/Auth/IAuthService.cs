namespace InkloomApi.Services;

public interface IAuthService
{
    Task<ServiceResponse<UserResponse>> Register(RegisterRequest userData);
    Task<ServiceResponse<LoginResponse>> Login(LoginRequest credentials);
    Task<ServiceResponse<LoginResponse>> Refresh(RefreshRequest credentials);
}
