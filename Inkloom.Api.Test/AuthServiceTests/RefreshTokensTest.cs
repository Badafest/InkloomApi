using System.Net;
using Microsoft.IdentityModel.Tokens;

namespace Inkloom.Api.Test;

public partial class AuthServiceInkloom.Api.Test
{
    [Fact, TestCasePriority(10)]
public async void RefreshValidTokensReturnsNewTokens()
{
    var loginResponse = await authService.Login(new()
    {
        Email = testUser.Email,
        Password = testUser.Password
    });

    var serviceResponse = await authService.Refresh(new()
    {
        AccessToken = loginResponse?.Data?.AccessToken?.Value ?? "",
        RefreshToken = loginResponse?.Data?.RefreshToken?.Value ?? ""
    });

    Assert.True(serviceResponse?.Success);
    Assert.Equal(serviceResponse?.Status, HttpStatusCode.OK);

    Assert.NotNull(serviceResponse?.Data?.AccessToken?.Value);
    Assert.NotEmpty(serviceResponse?.Data?.AccessToken?.Value ?? "");
    Assert.True(serviceResponse?.Data?.AccessToken?.Expiry > DateTime.UtcNow);

    Assert.NotNull(serviceResponse?.Data?.RefreshToken?.Value);
    Assert.NotEmpty(serviceResponse?.Data?.RefreshToken?.Value ?? "");
    Assert.True(serviceResponse?.Data?.RefreshToken?.Expiry > DateTime.UtcNow);
}

[Fact, TestCasePriority(11)]
public async void RefreshInvalidTokensThrowsSecurityTokenException()
{
    var refreshTask = authService.Refresh(new()
    {
        AccessToken = "SomeRandomAccessTokenValue",
        RefreshToken = "SomeRandomRefreshTokenValue"
    });

    await Assert.ThrowsAsync<SecurityTokenException>(() => refreshTask);
}

[Fact, TestCasePriority(12)]
public async void RefreshExpiredTokenThrowsSecurityTokenException()
{
    var refreshTask = authService.Refresh(new()
    {
        AccessToken = tokenService.GenerateJWT(testUser.Email, testUser.Username, DateTime.UtcNow.AddHours(2)),
        RefreshToken = tokenService.GenerateJWT(testUser.Email, testUser.Username, DateTime.UtcNow.AddMilliseconds(1))
    });

    await Assert.ThrowsAsync<SecurityTokenException>(() => refreshTask);
}

}