using Inkloom.Api.Assets;
using Microsoft.AspNetCore.Authorization;

namespace Inkloom.Api.Controllers;
[ApiController]
[Route(DEFAULT_ROUTE)]

[Authorize]
public class UserController(IUserService userService, IAuthService authService, IAssetManager assetManager, IConfiguration configuration) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private readonly IAuthService _authService = authService;
    private readonly IAssetManager _assetManager = assetManager;
    private readonly IConfiguration _configuration = configuration;

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
    public async Task<ActionResult<ServiceResponse<UserResponse>>> Update([FromForm] UpdateUserRequest updateData, IFormFile avatarImage)
    {
        if (avatarImage?.Length > 0)
        {
            var asset = ((List<Asset>)HttpContext.Items["Assets"]!)[0];
            var avatarId = _assetManager.AddAsset(asset.Record.FilePath, asset);
            updateData.Avatar = $"{_configuration["ApiBaseUrl"]}/asset/{avatarId}{Path.GetExtension(asset.Record.Name)}";
        }
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
