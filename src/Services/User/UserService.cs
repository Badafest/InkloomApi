using System.Net;

namespace InkloomApi.Services;
public class UserService(IMapper mapper, DataContext context, IAuthService authService) : IUserService
{

    private readonly IMapper _mapper = mapper;

    private readonly DataContext _context = context;

    private readonly IAuthService _authService = authService;

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
        var user = await _authService.VerifyOTP(updateData.Token, TokenType.EmailVerification, updateData.Email);
        if (user == null)
        {
            return new(HttpStatusCode.BadRequest) { Message = "Invalid Token or Email" };
        }
        user.EmailVerified = true;
        await _context.SoftSaveChangesAsync();
        return new() { Data = _mapper.Map<UserResponse>(user) };
    }
}