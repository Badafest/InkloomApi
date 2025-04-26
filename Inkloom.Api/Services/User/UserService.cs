using System.Net;
using Inkloom.Api.Assets;

namespace Inkloom.Api.Services;
public class UserService(IMapper mapper, DataContext context, IAuthService authService, IConfiguration configuration, IAssetManager assetManager) : IUserService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IMapper _mapper = mapper;
    private readonly DataContext _context = context;
    private readonly IAuthService _authService = authService;
    private readonly IAssetManager _assetManager = assetManager;

    public async Task<ServiceResponse<bool>> CheckUsername(string username)
    {
        var user = await _context.Users.FirstOrDefaultAsync(user => user.Username == username);
        return new() { Data = user?.Username != username };
    }

    public async Task<ServiceResponse<UserResponse>> DeleteUser(string username)
    {
        var user = await _context.Users.FirstOrDefaultAsync(user => user.Username == username);
        if (user == null || user.Username != username)
        {
            return new(HttpStatusCode.NotFound) { Message = "User not Found" };
        }
        _context.Remove(user);
        await _context.SoftSaveChangesAsync();
        return new() { Data = _mapper.Map<UserResponse>(user) };
    }

    public async Task<ServiceResponse<UserResponse>> GetUser(string username)
    {
        var user = await _context.Users.FirstOrDefaultAsync(user => user.Username == username);
        if (user == null || user.Username != username)
        {
            return new(HttpStatusCode.NotFound) { Message = "User not Found" };
        }
        return new() { Data = _mapper.Map<UserResponse>(user) };
    }

    public async Task<ServiceResponse<UserResponse>> UpdateUser(string username, UpdateUserRequest updateData)
    {
        var user = await _context.Users.FirstOrDefaultAsync(user => user.Username == username);
        if (user == null || user.Username != username)
        {
            return new(HttpStatusCode.NotFound) { Message = "User not Found" };
        }

        // check if the old avatar is served by inkloom and the new one is different from it
        // in that case delete the old avatar as it is no longer used
        var isInkloomAvatar = user.Avatar?.StartsWith(_configuration["ApiBaseUrl"]!) ?? false;
        if (!string.IsNullOrEmpty(updateData.Avatar) && user.Avatar != updateData.Avatar && isInkloomAvatar)
        {
            var assetId = user.Avatar!.Split("/")[^1].Split(".")[0];
            _assetManager.RemoveAsset(assetId);
        }

        user.About = updateData.About ?? user.About;
        user.Avatar = updateData.Avatar ?? user.Avatar;
        await _context.SoftSaveChangesAsync();
        return new() { Data = _mapper.Map<UserResponse>(user) };
    }

    public async Task<ServiceResponse<UserResponse>> ChangePassword(ChangePasswordRequest updateData)
    {
        var user = await _authService.VerifyOTP(updateData.Token, TokenType.PasswordReset, updateData.Email);
        if (user == null)
        {
            return new(HttpStatusCode.BadRequest) { Message = "Invalid Token or Email" };
        }
        user.Password = updateData.Password;
        await _context.SoftSaveChangesAsync();
        return new() { Data = _mapper.Map<UserResponse>(user) };
    }

    public async Task<ServiceResponse<UserResponse?>> VerifyEmail(VerifyEmailRequest updateData)
    {
        var user = await _authService.VerifyOTP(updateData?.Token ?? "", TokenType.EmailVerification, updateData?.Email ?? "");
        if (user == null)
        {
            return new(HttpStatusCode.BadRequest) { Message = "Invalid Token or Email" };
        }
        user.EmailVerified = true;
        await _context.SoftSaveChangesAsync();
        return new() { Data = _mapper.Map<UserResponse>(user) };
    }
}