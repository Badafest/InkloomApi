using System.Net;
using InkloomApi.Dtos;

namespace test;

public partial class AuthServiceTests
{

    [Theory, TestCasePriority(1)]
    [InlineData("")]
    [InlineData("@user")]
    [InlineData("user_name")]

    public async void RegisterBadUsernameThrowsArgumentException(string username)
    {
        var userData = new RegisterRequest()
        {
            Username = username,
            Email = Configuration.validUser.Email,
            Password = Configuration.validUser.Password
        };
        await Assert.ThrowsAsync<ArgumentException>(() => authService.Register(userData));
    }


    [Theory, TestCasePriority(2)]
    [InlineData("")]
    [InlineData("@user.com")]
    [InlineData("testuser.com")]

    public async void RegisterBadEmailThrowsArgumentException(string email)
    {
        var userData = new RegisterRequest()
        {
            Username = Configuration.validUser.Username,
            Email = email,
            Password = Configuration.validUser.Password
        };
        await Assert.ThrowsAsync<ArgumentException>(() => authService.Register(userData));
    }

    [Theory, TestCasePriority(3)]
    [InlineData("1!@#1#$!#$!")]
    [InlineData("password")]
    [InlineData("P!2A")]

    public async void RegisterBadPasswordThrowsArgumentException(string password)
    {
        var userData = new RegisterRequest()
        {
            Username = Configuration.validUser.Username,
            Email = Configuration.validUser.Email,
            Password = password
        };
        await Assert.ThrowsAsync<ArgumentException>(() => authService.Register(userData));
    }


    [Fact, TestCasePriority(4)]
    public async void RegisterValidUserReturnsUserResponse()
    {
        var userData = new RegisterRequest()
        {
            Username = Configuration.validUser.Username,
            Email = Configuration.validUser.Email,
            Password = Configuration.validUser.Password
        };
        var serviceResponse = await authService.Register(userData);

        Assert.True(serviceResponse?.Success);
        Assert.Equal(serviceResponse?.Status, HttpStatusCode.OK);
        Assert.Equal(serviceResponse?.Data?.Email, Configuration.validUser.Email);
        Assert.Equal(serviceResponse?.Data?.Username, Configuration.validUser.Username);
    }

    [Fact, TestCasePriority(5)]
    public async void RegisterExistingUsernameReturnsBadRequest()
    {
        var userData = new RegisterRequest()
        {
            Username = Configuration.validUser.Username,
            Email = "test2@mail.com",
            Password = Configuration.validUser.Password
        };
        var serviceResponse = await authService.Register(userData);

        Assert.False(serviceResponse?.Success);
        Assert.Equal(serviceResponse?.Status, HttpStatusCode.BadRequest);
    }


    [Fact, TestCasePriority(6)]
    public async void RegisterExistingEmailReturnsBadRequest()
    {
        var userData = new RegisterRequest()
        {
            Username = "test456",
            Email = Configuration.validUser.Email,
            Password = Configuration.validUser.Password
        };
        var serviceResponse = await authService.Register(userData);

        Assert.False(serviceResponse?.Success);
        Assert.Equal(serviceResponse?.Status, HttpStatusCode.BadRequest);
    }
}