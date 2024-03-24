using InkloomApi.Models;
using InkloomApi.Services;

namespace test;

[TestCaseOrderer(ordererTypeName: "test.TestCaseOrderer", ordererAssemblyName: "test")]
[Collection("Database Collection")]
// [TestCollectionPriority(1)]
public partial class AuthServiceTests(Configuration configuration)
{
    private static readonly TokenService tokenService = new(Configuration.config);
    private readonly AuthService authService = new(Configuration.config, configuration.dataContext, configuration.autoMapper, tokenService);
    public static readonly User testUser = new()
    {
        // A valid username can contain lowercase letters and numbers only
        Username = "test123",
        // A valid password is at least 8 characters long and contains at least 1 uppercase, 1 lowercase and 1 number each
        Password = "Str0ngPassword",
        Email = "test@mail.com"
    };
}