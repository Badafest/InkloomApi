using Microsoft.AspNetCore.Authorization;

namespace InkloomApi.Controllers
{
    [ApiController]
    [Route(DEFAULT_ROUTE)]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _AuthService;
        public AuthController(IAuthService AuthService)
        {
            _AuthService = AuthService;
        }
        [HttpGet]
        [Authorize]
        public ActionResult<ServiceResponse<UserResponse>> Me()
        {
            var username = User.Identity?.Name ?? string.Empty;
            var serviceResponse = _AuthService.Me(username);
            return StatusCode((int)serviceResponse.Status, serviceResponse);
        }

        [HttpPost]

        public async Task<ActionResult<ServiceResponse<LoginResponse>>> Login(LoginRequest credentials)
        {
            var serviceResponse = await _AuthService.Login(credentials);
            return StatusCode((int)serviceResponse.Status, serviceResponse);
        }

        [HttpPost]

        public async Task<ActionResult<ServiceResponse<LoginResponse>>> Refresh(RefreshRequest credentials)
        {
            var serviceResponse = await _AuthService.Refresh(credentials);
            return StatusCode((int)serviceResponse.Status, serviceResponse);
        }
    }
}