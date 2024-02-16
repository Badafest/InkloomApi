using Microsoft.AspNetCore.Authorization;

namespace InkloomApi.Controllers
{
    [ApiController]
    [Route(DEFAULT_ROUTE)]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]

        public async Task<ActionResult<ServiceResponse<LoginResponse>>> Login(LoginRequest credentials)
        {
            var serviceResponse = await _authService.Login(credentials);
            return StatusCode((int)serviceResponse.Status, serviceResponse);
        }

        [HttpPost]

        public async Task<ActionResult<ServiceResponse<LoginResponse>>> Refresh(RefreshRequest credentials)
        {
            var serviceResponse = await _authService.Refresh(credentials);
            return StatusCode((int)serviceResponse.Status, serviceResponse);
        }
    }
}