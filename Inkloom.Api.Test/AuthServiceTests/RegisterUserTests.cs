using System.Net;
using Inkloom.Api.Data.Models;
using Inkloom.Api.Dtos;

namespace Inkloom.Api.Test;

public partial class AuthServiceTests
{
    private readonly static string newUserPassword = "Str0ngPassword";
    private readonly User newUser = new()
    {
        // A valid username can contain lowercase letters and numbers only
        Username = "test123",
        Email = "test@mail.com",
        // A valid password is at least 8 characters long and contains at least 1 uppercase, 1 lowercase and 1 number each
        Password = newUserPassword
    };


    [Theory]
    [InlineData("")]
    [InlineData("@user")]
    [InlineData("user_name")]

    public async void RegisterBadUsernameThrowsArgumentException(string username)
    {
        var userData = new RegisterRequest()
        {
            Username = username,
            Email = newUser.Email,
            Password = newUserPassword
        };
        await Assert.ThrowsAsync<ArgumentException>(() => authService.Register(userData));
    }


    [Theory]
    [InlineData("")]
    [InlineData("@user.com")]
    [InlineData("testuser.com")]

    public async void RegisterBadEmailThrowsArgumentException(string email)
    {
        var userData = new RegisterRequest()
        {
            Username = newUser.Username,
            Email = email,
            Password = newUserPassword
        };
        await Assert.ThrowsAsync<ArgumentException>(() => authService.Register(userData));
    }

    [Theory]
    [InlineData("1!@#1#$!#$!")]
    [InlineData("password")]
    [InlineData("P!2A")]

    public async void RegisterBadPasswordThrowsArgumentException(string password)
    {
        var userData = new RegisterRequest()
        {
            Username = newUser.Username,
            Email = newUser.Email,
            Password = password
        };
        await Assert.ThrowsAsync<ArgumentException>(() => authService.Register(userData));
    }


    [Fact]
    public async void RegisterValidUserReturnsUserResponse()
    {
        var userData = new RegisterRequest()
        {
            Username = newUser.Username,
            Email = newUser.Email,
            Password = newUserPassword
        };
        var serviceResponse = await authService.Register(userData);

        Assert.True(serviceResponse?.Success);
        Assert.Equal(serviceResponse?.Status, HttpStatusCode.OK);
        Assert.Equal(serviceResponse?.Data?.Email, userData.Email);
        Assert.Equal(serviceResponse?.Data?.Username, userData.Username);
    }

    [Fact]
    public async void RegisterExistingUsernameReturnsBadRequest()
    {
        var userData = new RegisterRequest()
        {
            Username = testUser.Username,
            Email = "test2@mail.com",
            Password = testUserPassword
        };
        var serviceResponse = await authService.Register(userData);

        Assert.False(serviceResponse?.Success);
        Assert.Equal(serviceResponse?.Status, HttpStatusCode.BadRequest);
    }


    [Fact]
    public async void RegisterExistingEmailReturnsBadRequest()
    {
        var userData = new RegisterRequest()
        {
            Username = "test456",
            Email = testUser.Email,
            Password = testUserPassword
        };
        var serviceResponse = await authService.Register(userData);

        Assert.False(serviceResponse?.Success);
        Assert.Equal(serviceResponse?.Status, HttpStatusCode.BadRequest);
    }
}