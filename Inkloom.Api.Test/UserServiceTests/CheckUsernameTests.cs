namespace Inkloom.Api.Test;

public partial class UserServiceInkloom.Api.Test
{

    [Fact, TestCasePriority(15)]

public async void CheckExistingUsernameReturnsFalse()
{
    var serviceResponse = await userService.CheckUsername(testUser.Username);

    Assert.False(serviceResponse?.Data);
}

[Fact, TestCasePriority(16)]
public async void CheckNonExistingUsernameReturnsTrue()
{
    var serviceResponse = await userService.CheckUsername(testUser.Username + "1423");

    Assert.True(serviceResponse?.Data);
}
}