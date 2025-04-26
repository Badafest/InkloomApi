namespace Inkloom.Api.Test;

public partial class UserServiceTests
{

    [Fact]

    public async Task CheckExistingUsernameReturnsFalse()
    {
        var serviceResponse = await userService.CheckUsername(testUser.Username);

        Assert.False(serviceResponse?.Data);
    }

    [Fact]
    public async Task CheckNonExistingUsernameReturnsTrue()
    {
        var serviceResponse = await userService.CheckUsername(testUser.Username + "1423");

        Assert.True(serviceResponse?.Data);
    }
}