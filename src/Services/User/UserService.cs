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

    public async Task<ServiceResponse<UserResponse>> GetUser(string username)
    {
        var user = await _context.Users.FirstOrDefaultAsync(user => user.Username == username);
        if (user == null || user.Username != username)
        {
            return new(HttpStatusCode.NotFound) { Message = "User not Found" };
        }
        return new() { Data = _mapper.Map<UserResponse>(user) };
    }

}