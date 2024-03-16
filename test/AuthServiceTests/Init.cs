using InkloomApi.Models;
using InkloomApi.Services;

namespace test;

[TestCaseOrderer(ordererTypeName: "test.PriorityOrderer", ordererAssemblyName: "test")]
[Collection("AuthServiceTests")]
public partial class AuthServiceTests
{
    private static readonly User validUser = new()
    {
        // A valid username can contain lowercase letters and numbers only
        Username = "test123",
        // A valid password is at least 8 characters long and contains at least 1 uppercase, 1 lowercase and 1 number each
        Password = "Str0ngPassword",
        Email = "test@mail.com"
    };
    public static readonly AuthService authService = new(Configuration.config, Configuration.dataContext, Configuration.autoMapper);

}