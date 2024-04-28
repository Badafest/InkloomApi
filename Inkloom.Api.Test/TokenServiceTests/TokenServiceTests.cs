using Inkloom.Api.Models;
using Inkloom.Api.Services;

namespace Inkloom.Api.Test;

// [TestCaseOrderer(ordererTypeName: "Inkloom.Api.Test.TestCaseOrderer", ordererAssemblyName: "Inkloom.Api.Test")]
[Collection("Database Collection")]
// [TestCollectionPriority(2)]
public partial class TokenServiceTests(ITokenService tokenService)
{
    public static readonly User testUser = new()
    {
        Username = "user",
        Password = "Str0ngPassword123",
        Email = "user@inkloom.com"
    };
}