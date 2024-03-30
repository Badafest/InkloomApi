using System.Net;

namespace InkloomApi.Services;
public class UserService(IMapper mapper, DataContext context) : IUserService
{

    private readonly IMapper _mapper = mapper;

    private readonly DataContext _context = context;

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
        await _context.SoftSaveChangesAsync(user.Id);
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
        user.Password = updateData.Password ?? user.Password;
        await _context.SoftSaveChangesAsync(user.Id);
        return new() { Data = _mapper.Map<UserResponse>(user) };
    }
}