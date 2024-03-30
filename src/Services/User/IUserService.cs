namespace InkloomApi.Services;
public interface IUserService
{
    Task<ServiceResponse<UserResponse>> GetUser(string username);
    Task<ServiceResponse<UserResponse>> UpdateUser(string username, UpdateUserRequest updateData);
    Task<ServiceResponse<UserResponse>> DeleteUser(string username);
    Task<ServiceResponse<bool>> CheckUsername(string username);
}