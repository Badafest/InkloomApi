using System.Net;
using Inkloom.Api.Dtos;

namespace Inkloom.Api.Test;

public partial class AuthServiceInkloom.Api.Test
{
    [Theory, TestCasePriority(7)]
[InlineData("")]
[InlineData("user@mail.com")]
[InlineData("test154")]

public async void LoginInvalidEmailReturnsBadRequest(string email)
{
    var userData = new LoginRequest()
    {
        Email = email,
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
        Email = testUser.Email,
        Password = password
    };
    var serviceResponse = await authService.Login(userData);

    Assert.False(serviceResponse?.Success);
    Assert.Equal(serviceResponse?.Status, HttpStatusCode.BadRequest);
}


[Fact, TestCasePriority(9)]
public async void LoginValidUserReturnsLoginResponse()
{
    var userData = new LoginRequest()
    {
        Email = testUser.Email,
        Password = testUser.Password
    };
    var serviceResponse = await authService.Login(userData);
    Assert.Null(serviceResponse?.Message);
    Assert.True(serviceResponse?.Success);
    Assert.Equal(serviceResponse?.Status, HttpStatusCode.OK);
    Assert.Equal(serviceResponse?.Data?.Username, testUser.Username);
}
}