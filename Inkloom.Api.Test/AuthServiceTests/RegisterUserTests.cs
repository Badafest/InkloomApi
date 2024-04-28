using System.Net;
using Inkloom.Api.Dtos;

namespace Inkloom.Api.Test;

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
            Email = testUser.Email,
            Password = testUser.Password
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
            Username = testUser.Username,
            Email = email,
            Password = testUser.Password
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
            Username = testUser.Username,
            Email = testUser.Email,
            Password = password
        };
        await Assert.ThrowsAsync<ArgumentException>(() => authService.Register(userData));
    }


    [Fact, TestCasePriority(4)]
    public async void RegisterValidUserReturnsUserResponse()
    {
        var userData = new RegisterRequest()
        {
            Username = testUser.Username,
            Email = testUser.Email,
            Password = testUser.Password
        };
        var serviceResponse = await authService.Register(userData);

        Assert.True(serviceResponse?.Success);
        Assert.Equal(serviceResponse?.Status, HttpStatusCode.OK);
        Assert.Equal(serviceResponse?.Data?.Email, testUser.Email);
        Assert.Equal(serviceResponse?.Data?.Username, testUser.Username);
    }

    [Fact, TestCasePriority(5)]
    public async void RegisterExistingUsernameReturnsBadRequest()
    {
        var userData = new RegisterRequest()
        {
            Username = testUser.Username,
            Email = "test2@mail.com",
            Password = testUser.Password
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
            Email = testUser.Email,
            Password = testUser.Password
        };
        var serviceResponse = await authService.Register(userData);

        Assert.False(serviceResponse?.Success);
        Assert.Equal(serviceResponse?.Status, HttpStatusCode.BadRequest);
    }
}