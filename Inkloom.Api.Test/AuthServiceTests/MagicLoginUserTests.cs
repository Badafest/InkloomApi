using System.Net;
using Inkloom.Api.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Inkloom.Api.Test;

public partial class AuthServiceTests
{
    [Fact, TestCasePriority(13)]
    public async void MagicLoginWithValidEmailGeneratesOTPAndCanLogin()
    {
        var serviceResponse = await authService.GenerateAndSendMagicToken(testUser.Email);
        Assert.True(serviceResponse?.Success);
        var token = await dataContext.Tokens.FirstOrDefaultAsync(token => token.Type == TokenType.MagicLink && token.UserId == testUser.Id);
        var verifyResponse = await authService.MagicLogin(token?.Value ?? "");
        Assert.True(verifyResponse?.Success);
        Assert.Equal(verifyResponse?.Status, HttpStatusCode.OK);
        Assert.Equal(verifyResponse?.Data?.Username, testUser.Username);
    }

    [Theory, TestCasePriority(14)]
    [InlineData("xutrwlasdas@mail.com")]
    public async void MagicLoginWithInvalidEmailDoesnotGenerateOTP(string testEmail)
    {
        var serviceResponse = await authService.GenerateAndSendMagicToken(testEmail);
        Assert.True(serviceResponse?.Success);
        var token = await dataContext.Tokens.Include(token => token.User).FirstOrDefaultAsync(token => token.Type == TokenType.MagicLink && token.User != null && token.User.Email == testEmail);
        Assert.Empty(token?.Value ?? "");
    }


    [Theory, TestCasePriority(15)]
    [InlineData("UZX12345")]
    public async void MagicLoginWithInvalidOTPThrowsSecurityException(string otp)
    {
        var serviceResponse = authService.MagicLogin(otp);
        await Assert.ThrowsAsync<SecurityTokenException>(() => serviceResponse);
    }
}