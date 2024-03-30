using System.Net;

namespace test;

public partial class UserServiceTests
{

    [Theory, TestCasePriority(17)]
    [InlineData("weakpassword")]
    [InlineData("12@1Sa")]

    public async void UpdateUserWithWeakPasswordThrowsArgumentException(string weakPassword)
    {
        var updateService = userService.UpdateUser(new()
        {
            Id = testUser.Id,
            Password = weakPassword
        });

        await Assert.ThrowsAsync<ArgumentException>(() => updateService);
    }

    [Fact, TestCasePriority(18)]
    public async void UpdateUserWithStrongPasswordReturnsUserResponse()
    {
        var serviceResponse = await userService.UpdateUser(new()
        {
            Id = testUser.Id,
            Password = "Str0ngP@ssword123"
        });

        Assert.True(serviceResponse?.Success);
        Assert.Equal(testUser.Username, serviceResponse?.Data?.Username);
    }

    [Fact, TestCasePriority(19)]
    public async void UpdateAboutAndAvatarReturnsUserResponse()
    {
        var about = "I am a handsome user";
        var avatar = "https://i.natgeofe.com/n/548467d8-c5f1-4551-9f58-6817a8d2c45e/NationalGeographic_2572187_square.jpg";
        var serviceResponse = await userService.UpdateUser(new()
        {
            Id = testUser.Id,
            About = about,
            Avatar = avatar
        });

        Assert.True(serviceResponse?.Success);
        Assert.Equal(testUser.Username, serviceResponse?.Data?.Username);
        Assert.Equal(about, serviceResponse?.Data?.About);
        Assert.Equal(avatar, serviceResponse?.Data?.Avatar);
    }
}