namespace Inkloom.Api.Services;
public interface IUserService
{
    Task<ServiceResponse<UserResponse>> GetUser(string username);
    Task<ServiceResponse<UserResponse>> UpdateUser(string username, UpdateUserRequest updateData);
    Task<ServiceResponse<UserResponse>> ChangePassword(ChangePasswordRequest updateData);
    Task<ServiceResponse<UserResponse?>> VerifyEmail(string token, string email);
    Task<ServiceResponse<UserResponse>> DeleteUser(string username);
    Task<ServiceResponse<bool>> CheckUsername(string username);
}