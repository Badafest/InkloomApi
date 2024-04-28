namespace Inkloom.Api.Test;

public partial class TokenServiceTests
{
    [Theory]
    [InlineData(6)]
    [InlineData(8)]
    public void GetValidUserReturnsUserResponse(short length)
    {
        var otp = tokenService.GenerateOTP(length);

        Assert.NotNull(otp);
        Assert.Equal(length, otp.Length);
    }
}