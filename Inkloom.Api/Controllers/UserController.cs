using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace Inkloom.Api.Controllers;
[ApiController]
[Route(DEFAULT_ROUTE)]

[Authorize]
public class UserController(IUserService userService, IAuthService authService) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private readonly IAuthService _authService = authService;


    [HttpGet]
    public async Task<ActionResult<ServiceResponse<UserResponse>>> Me()
    {
        var username = User.Identity?.Name ?? "";
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

    [HttpPatch]
    public async Task<ActionResult<ServiceResponse<UserResponse>>> Update(UpdateUserRequest updateData)
    {
        var username = User.Identity?.Name ?? "";
        var serviceResponse = await _userService.UpdateUser(username, updateData);
        return StatusCode((int)serviceResponse.Status, serviceResponse);
    }

    [HttpPatch("Change-Password")]
    [AllowAnonymous]
    public async Task<ActionResult<ServiceResponse<UserResponse>>> ChangePassword(ChangePasswordRequest updateData)
    {
        var serviceResponse = await _userService.ChangePassword(updateData);
        return StatusCode((int)serviceResponse.Status, serviceResponse);
    }

    [HttpPost("Verify-Email")]
    [AllowAnonymous]
    public async Task<ActionResult<ServiceResponse<UserResponse?>>> VerifyEmail(VerifyEmailRequest updateData)
    {
        var serviceResponse = updateData?.Token != null ?
            await _userService.VerifyEmail(updateData) :
            await _authService.GenerateAndSendOTP(updateData?.Email ?? "", TokenType.EmailVerification);
        return StatusCode((int)serviceResponse.Status, serviceResponse);
    }

    [HttpDelete]
    public async Task<ActionResult<ServiceResponse<UserResponse>>> Delete()
    {
        var username = User.Identity?.Name ?? "";
        var serviceResponse = await _userService.DeleteUser(username);
        return StatusCode((int)serviceResponse.Status, serviceResponse);
    }
}
