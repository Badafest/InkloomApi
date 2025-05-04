namespace Inkloom.Api.Dtos;
public class UpdateUserRequest

{
    public string? About { get; set; }
    public string? Avatar { get; set; }
    public string? DisplayName { get; set; }

}

public class ChangePasswordRequest
{
    public string Token { get; set; } = "";

    public string Email { get; set; } = "";

    public string Password { get; set; } = "";
}

public class VerifyEmailRequest
{
    public string? Token { get; set; }

}

