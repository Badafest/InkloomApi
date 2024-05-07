namespace Inkloom.Api.Test;

public partial class UserServiceTests
{

    [Fact]

    public async void CheckExistingUsernameReturnsFalse()
    {
        var serviceResponse = await userService.CheckUsername(testUser.Username);

        Assert.False(serviceResponse?.Data);
    }

    [Fact]
    public async void CheckNonExistingUsernameReturnsTrue()
    {
        var serviceResponse = await userService.CheckUsername(testUser.Username + "1423");

        Assert.True(serviceResponse?.Data);
    }
}