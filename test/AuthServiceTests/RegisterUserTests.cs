using System.Net;
using InkloomApi.Dtos;
using InkloomApi.Services;

namespace test;

[TestCaseOrderer(ordererTypeName: "test.PriorityOrderer", ordererAssemblyName: "test")]
public class RegisterUserTests
{
    // A valid username can contain lowercase letters and numbers only
    private readonly string VALID_USERNAME = "test123";
    // A valid password is at least 8 characters long and contains at least 1 uppercase, 1 lowercase and 1 number each
    private readonly string VALID_PASSWORD = "Str0ngPassword";
    private readonly string VALID_EMAIL = "test@mail.com";

    [Theory, TestPriority(1)]
    [InlineData("")]
    [InlineData("@user")]
    [InlineData("user_name")]

    public async void RegisterBadUsernameThrowsArgumentException(string username)
    {
        var authService = new AuthService(Configuration.config, Configuration.dataContext, Configuration.autoMapper);
        var userData = new RegisterRequest() { Username = username, Email = VALID_EMAIL, Password = VALID_PASSWORD };
        await Assert.ThrowsAsync<ArgumentException>(() => authService.Register(userData));
    }


    [Theory, TestPriority(2)]
    [InlineData("")]
    [InlineData("@user.com")]
    [InlineData("testuser.com")]

    public async void RegisterBadEmailThrowsArgumentException(string email)
    {
        var authService = new AuthService(Configuration.config, Configuration.dataContext, Configuration.autoMapper);
        var userData = new RegisterRequest() { Username = VALID_USERNAME, Email = email, Password = VALID_PASSWORD };
        await Assert.ThrowsAsync<ArgumentException>(() => authService.Register(userData));
    }

    [Theory, TestPriority(3)]
    [InlineData("1!@#1#$!#$!")]
    [InlineData("password")]
    [InlineData("P!2A")]

    public async void RegisterBadPasswordThrowsArgumentException(string password)
    {
        var authService = new AuthService(Configuration.config, Configuration.dataContext, Configuration.autoMapper);
        var userData = new RegisterRequest() { Username = VALID_USERNAME, Email = VALID_EMAIL, Password = password };
        await Assert.ThrowsAsync<ArgumentException>(() => authService.Register(userData));
    }


    [Fact, TestPriority(4)]
    public async void RegisterValidUserReturnsUserResponse()
    {
        var authService = new AuthService(Configuration.config, Configuration.dataContext, Configuration.autoMapper);
        var userData = new RegisterRequest() { Username = VALID_USERNAME, Email = VALID_EMAIL, Password = VALID_PASSWORD };
        var serviceResponse = await authService.Register(userData);

        Assert.True(serviceResponse?.Success);
        Assert.Equal(serviceResponse?.Status, HttpStatusCode.OK);
        Assert.Equal(serviceResponse?.Data?.Email, VALID_EMAIL);
        Assert.Equal(serviceResponse?.Data?.Username, VALID_USERNAME);
    }

    [Fact, TestPriority(5)]
    public async void RegisterExistingUsernameReturnsBadRequest()
    {
        var authService = new AuthService(Configuration.config, Configuration.dataContext, Configuration.autoMapper);
        var userData = new RegisterRequest() { Username = VALID_USERNAME, Email = "test2@mail.com", Password = VALID_PASSWORD };
        var serviceResponse = await authService.Register(userData);

        Assert.False(serviceResponse?.Success);
        Assert.Equal(serviceResponse?.Status, HttpStatusCode.BadRequest);
    }


    [Fact, TestPriority(6)]
    public async void RegisterExistingEmailReturnsBadRequest()
    {
        var authService = new AuthService(Configuration.config, Configuration.dataContext, Configuration.autoMapper);
        var userData = new RegisterRequest() { Username = "test456", Email = VALID_EMAIL, Password = VALID_PASSWORD };
        var serviceResponse = await authService.Register(userData);

        Assert.False(serviceResponse?.Success);
        Assert.Equal(serviceResponse?.Status, HttpStatusCode.BadRequest);
    }
}