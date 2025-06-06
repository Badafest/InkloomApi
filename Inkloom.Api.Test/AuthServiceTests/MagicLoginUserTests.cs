using System.Net;
using Inkloom.Api.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Inkloom.Api.Test;

public partial class AuthServiceTests
{
    [Fact]
    public async Task MagicLoginWithValidEmailGeneratesOTPAndCanLogin()
    {
        var serviceResponse = await authService.GenerateAndSendMagicToken(testUser.Email);
        Assert.True(serviceResponse?.Success);
        var token = await dataContext.Tokens.FirstOrDefaultAsync(
            token => token.Type == TokenType.MagicLink && token.User != null && token.User.Email == testUser.Email, TestContext.Current.CancellationToken);
        Assert.NotNull(token);
        Assert.NotEqual(0, token.Value.Length);
        var verifyResponse = await authService.MagicLogin(token?.Value ?? "");
        Assert.True(verifyResponse?.Success);
        Assert.Equal(verifyResponse?.Status, HttpStatusCode.OK);
        Assert.Equal(verifyResponse?.Data?.Username, testUser.Username);
    }

    [Theory]
    [InlineData("xutrwlasdas@mail.com")]
    public async Task MagicLoginWithInvalidEmailDoesnotGenerateOTP(string testEmail)
    {
        var serviceResponse = await authService.GenerateAndSendMagicToken(testEmail);
        Assert.True(serviceResponse?.Success);
        var token = await dataContext.Tokens.FirstOrDefaultAsync(
            token => token.Type == TokenType.MagicLink && token.User != null && token.User.Email == testEmail, TestContext.Current.CancellationToken);
        Assert.Empty(token?.Value ?? "");
    }


    [Theory]
    [InlineData("UZX12345")]
    public async Task MagicLoginWithInvalidOTPThrowsSecurityException(string otp)
    {
        var serviceResponse = authService.MagicLogin(otp);
        await Assert.ThrowsAsync<SecurityTokenException>(() => serviceResponse);
    }
}