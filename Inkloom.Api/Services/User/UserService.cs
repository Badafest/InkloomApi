using System.Net;
using Inkloom.Api.Assets;
using Inkloom.Api.Extensions;

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
        var user = await _context.Users
                        .Where(user => user.Username == username)
                        .Include(user => user.Followers)
                        .Include(user => user.Followings)
                        .Include(user => user.Blogs)
                        .FirstOrDefaultAsync();

        if (user == null || user.Username != username)
        {
            return new(HttpStatusCode.NotFound) { Message = "User not Found" };
        }
        return new() { Data = _mapper.Map<UserResponse>(user) };
    }

    public async Task<ServiceResponse<UserResponse>> UpdateUser(string username, UpdateUserRequest updateData)
    {
        var user = await _context.Users
                        .Include(user => user.Followers)
                        .Include(user => user.Followings)
                        .Include(user => user.Blogs)
                        .FirstOrDefaultAsync(user => user.Username == username);

        if (user == null || user.Username != username)
        {
            return new(HttpStatusCode.NotFound) { Message = "User not Found" };
        }

        _assetManager.DeleteOldFile(_configuration["ApiBaseUrl"]!, user.Avatar ?? "", updateData.Avatar ?? "");

        user.About = updateData.About;
        user.Avatar = updateData.Avatar;
        user.DisplayName = updateData.DisplayName ?? "";

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

    public async Task<ServiceResponse<UserResponse?>> VerifyEmail(string token, string email)
    {
        var user = await _authService.VerifyOTP(token, TokenType.EmailVerification, email);
        if (user == null)
        {
            return new(HttpStatusCode.BadRequest) { Message = "Invalid Token or Email" };
        }
        user.EmailVerified = true;
        await _context.SoftSaveChangesAsync();
        return new() { Data = _mapper.Map<UserResponse>(user) };
    }
}