using Microsoft.AspNetCore.Authorization;

namespace InkloomApi.Controllers
{
    [ApiController]
    [Route(DEFAULT_ROUTE)]
    public class UserController : ControllerBase
    {
        private readonly IUserService _UserService;
        public UserController(IUserService UserService)
        {
            _UserService = UserService;
        }
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ServiceResponse<UserResponse>>> Me()
        {
            var username = User.Identity?.Name ?? string.Empty;
            var serviceResponse = await _UserService.GetUser(username);
            return StatusCode((int)serviceResponse.Status, serviceResponse);
        }
    }
}