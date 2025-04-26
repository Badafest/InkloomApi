using System.Net;
using Inkloom.Api.Dtos;

namespace Inkloom.Api.Test;

public partial class AuthServiceTests
{
    [Theory]
    [InlineData("")]
    [InlineData("user@mail.com")]
    [InlineData("test154")]

    public async Task LoginInvalidEmailReturnsBadRequest(string email)
    {
        var userData = new LoginRequest()
        {
            Email = email,
            Password = testUserPassword
        };
        var serviceResponse = await authService.Login(userData);

        Assert.False(serviceResponse?.Success);
        Assert.Equal(serviceResponse?.Status, HttpStatusCode.BadRequest);
    }


    [Theory]
    [InlineData("")]
    [InlineData("WeakPassword")]
    [InlineData("str0ngpassword")]

    public async Task LoginInvalidPasswordReturnsBadRequest(string password)
    {
        var userData = new LoginRequest()
        {
            Email = testUser.Email,
            Password = password
        };
        var serviceResponse = await authService.Login(userData);

        Assert.False(serviceResponse?.Success);
        Assert.Equal(serviceResponse?.Status, HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task LoginValidUserReturnsLoginResponse()
    {
        var userData = new LoginRequest()
        {
            Email = testUser.Email,
            Password = testUserPassword
        };
        var serviceResponse = await authService.Login(userData);
        Assert.Null(serviceResponse?.Message);
        Assert.True(serviceResponse?.Success);
        Assert.Equal(serviceResponse?.Status, HttpStatusCode.OK);
        Assert.Equal(serviceResponse?.Data?.Username, testUser.Username);
    }
}