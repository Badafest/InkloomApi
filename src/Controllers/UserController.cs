using Microsoft.AspNetCore.Authorization;

namespace InkloomApi.Controllers;
[ApiController]
[Route(DEFAULT_ROUTE)]

[Authorize]
public class UserController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet]
    [Route("Me")]
    public async Task<ActionResult<ServiceResponse<UserResponse>>> Me()
    {
        var username = User.Identity?.Name ?? string.Empty;
        var serviceResponse = await _userService.GetUser(username);
        return StatusCode((int)serviceResponse.Status, serviceResponse);
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("Check-Username")]
    public async Task<ActionResult<ServiceResponse<bool>>> CheckUsername(string username)
    {
        var serviceResponse = await _userService.CheckUsername(username);
        return StatusCode((int)serviceResponse.Status, serviceResponse);
    }
}
