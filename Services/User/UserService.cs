using System.Net;

namespace InkloomApi.Services
{
    public class UserService : IUserService
    {

        private readonly IMapper _mapper;

        private readonly DataContext _context;

        public UserService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
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
}