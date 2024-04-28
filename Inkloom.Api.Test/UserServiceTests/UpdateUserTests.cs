namespace Inkloom.Api.Test;

public partial class UserServiceTests
{

    [Fact, TestCasePriority(19)]
    public async void UpdateAboutAndAvatarReturnsUserResponse()
    {
        var about = "I am a handsome user";
        var avatar = "https://i.natgeofe.com/n/548467d8-c5f1-4551-9f58-6817a8d2c45e/NationalGeographic_2572187_square.jpg";

        var serviceResponse = await userService.UpdateUser(testUser.Username, new() { About = about, Avatar = avatar });

        Assert.True(serviceResponse?.Success);
        Assert.Equal(testUser.Username, serviceResponse?.Data?.Username);
        Assert.Equal(about, serviceResponse?.Data?.About);
        Assert.Equal(avatar, serviceResponse?.Data?.Avatar);
    }
}