using System.Net;


namespace Inkloom.Api.Test;

public partial class UserServiceTests
{

    [Theory]
    [InlineData("")]
    [InlineData("@user")]
    [InlineData("user_name")]

    public async void GetNonExistingUserReturnsNotFound(string username)
    {
        var serviceResponse = await userService.GetUser(username);

        Assert.False(serviceResponse?.Success);
        Assert.Equal(serviceResponse?.Status, HttpStatusCode.NotFound);
    }

    [Fact]
    public async void GetValidUserReturnsUserResponse()
    {
        var serviceResponse = await userService.GetUser(testUser.Username);

        Assert.True(serviceResponse?.Success);
        Assert.Equal(serviceResponse?.Data?.Username, testUser.Username);
    }
}