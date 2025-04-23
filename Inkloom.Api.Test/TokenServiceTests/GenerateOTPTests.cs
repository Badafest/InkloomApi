namespace Inkloom.Api.Test;

public partial class TokenServiceTests
{
    [Theory]
    [InlineData(6)]
    [InlineData(8)]
    public void GenerateOTPReturnsTokenOfDesiredLength(short length)
    {
        var otp = tokenService.GenerateOTP(length);

        Assert.NotNull(otp);
        Assert.Equal(length, otp.Length);
    }

    [Fact]
    public void GenerateJWTReturnsValidToken()
    {
        var jwt = tokenService.GenerateJWT(testUser.Email, testUser.Username, DateTime.UtcNow.AddMinutes(20));

        Assert.NotNull(jwt);

        var principal = tokenService.ValidateInkloomToken(jwt);

        Assert.Equal(testUser.Username, principal.Identity?.Name);
    }
}