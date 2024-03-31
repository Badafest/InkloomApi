namespace InkloomApi.Controllers;
[ApiController]
[Route(DEFAULT_ROUTE)]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost]
    [Route("Register")]
    public async Task<ActionResult<ServiceResponse<UserResponse>>> Register(RegisterRequest userData)
    {
        var serviceResponse = await _authService.Register(userData);
        return StatusCode((int)serviceResponse.Status, serviceResponse);
    }

    [HttpPost]
    [Route("Login")]
    public async Task<ActionResult<ServiceResponse<LoginResponse>>> Login(LoginRequest credentials)
    {
        var serviceResponse = await _authService.Login(credentials);
        return StatusCode((int)serviceResponse.Status, serviceResponse);
    }

    [HttpPost]
    [Route("Magic-Login")]
    public async Task<ActionResult<ServiceResponse<string?>>> MagicLogin(MagicLoginRequest credentials)
    {
        var serviceResponse = await _authService.GenerateAndSendOTP(credentials.Email, TokenType.MagicLink);
        return StatusCode((int)serviceResponse.Status, serviceResponse);
    }

    [HttpGet]
    [Route("Magic-Login")]
    public async Task<ActionResult<ServiceResponse<LoginResponse>>> MagicLogin(string token)
    {
        var serviceResponse = await _authService.MagicLogin(token);
        return StatusCode((int)serviceResponse.Status, serviceResponse);
    }

    [HttpPost]
    [Route("Refresh")]
    public async Task<ActionResult<ServiceResponse<LoginResponse>>> Refresh(RefreshRequest credentials)
    {
        var serviceResponse = await _authService.Refresh(credentials);
        return StatusCode((int)serviceResponse.Status, serviceResponse);
    }

    [HttpPost]
    [Route("Password")]
    public async Task<ActionResult<ServiceResponse<string?>>> ResetPassword(ResetPasswordRequest resetRequest)
    {
        var serviceResponse = await _authService.GenerateAndSendOTP(resetRequest.Email, TokenType.PasswordReset);
        return StatusCode((int)serviceResponse.Status, serviceResponse);
    }
}
