namespace InkloomApi.Services;
public interface IUserService
{
    Task<ServiceResponse<UserResponse>> GetUser(string username);
    Task<ServiceResponse<bool>> CheckUsername(string username);
}