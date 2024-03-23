using System.Net;
using InkloomApi.Dtos;

namespace test;

public partial class AuthServiceTests
{
    [Theory, TestCasePriority(7)]
    [InlineData("")]
    [InlineData("@user")]
    [InlineData("test154")]

    public async void LoginInvalidUsernameReturnsBadRequest(string username)
    {
        var userData = new LoginRequest()
        {
            Username = username,
            Password = testUser.Password
        };
        var serviceResponse = await authService.Login(userData);

        Assert.False(serviceResponse?.Success);
        Assert.Equal(serviceResponse?.Status, HttpStatusCode.BadRequest);
    }


    [Theory, TestCasePriority(8)]
    [InlineData("")]
    [InlineData("WeakPassword")]
    [InlineData("str0ngpassword")]

    public async void LoginInvalidPasswordReturnsBadRequest(string password)
    {
        var userData = new LoginRequest()
        {
            Username = testUser.Username,
            Password = password
        };
        var serviceResponse = await authService.Login(userData);

        Assert.False(serviceResponse?.Success);
        Assert.Equal(serviceResponse?.Status, HttpStatusCode.BadRequest);
    }


    [Fact, TestCasePriority(9)]
    public async void LoginValidUserReturnsUserResponse()
    {
        var userData = new LoginRequest()
        {
            Username = testUser.Username,
            Password = testUser.Password
        };
        var serviceResponse = await authService.Login(userData);
        Assert.Null(serviceResponse?.Message);
        Assert.True(serviceResponse?.Success);
        Assert.Equal(serviceResponse?.Status, HttpStatusCode.OK);
        Assert.Equal(serviceResponse?.Data?.Username, testUser.Username);
    }
}