namespace InkloomApi.Services
{
    public interface IAuthService
    {
        Task<ServiceResponse<LoginResponse>> Login(LoginRequest credentials);
        Task<ServiceResponse<LoginResponse>> Refresh(RefreshRequest credentials);
        ServiceResponse<UserResponse> Me(string username);
    }
}