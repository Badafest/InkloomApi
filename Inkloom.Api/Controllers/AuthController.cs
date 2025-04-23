namespace Inkloom.Api.Controllers;
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
    public async Task<ActionResult<ServiceResponse<LoginResponse?>>> MagicLogin(MagicLoginRequest credentials)
    {
        var serviceResponse = credentials.Token != null ?
            await _authService.MagicLogin(credentials.Token) : credentials.Email != null ?
            await _authService.GenerateAndSendMagicToken(credentials.Email) :
            new(System.Net.HttpStatusCode.BadRequest) { Message = "Email or Token is required" };

        return StatusCode((int)serviceResponse.Status, serviceResponse);
    }

    [HttpPost]
    [Route("SSO-Login")]
    public async Task<ActionResult<ServiceResponse<LoginResponse?>>> GoogleLogin(SsoLoginRequest credentials)
    {
        var serviceResponse = await _authService.SsoLogin(credentials.Token, credentials.Type);
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
    [Route("Forgot-Password")]
    public async Task<ActionResult<ServiceResponse<object?>>> ResetPassword(ResetPasswordRequest resetRequest)
    {
        var serviceResponse = await _authService.GenerateAndSendOTP(resetRequest.Email, TokenType.PasswordReset);
        return StatusCode((int)serviceResponse.Status, serviceResponse);
    }
}
