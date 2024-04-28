using Inkloom.Api.Models;
using Inkloom.Api.Services;

namespace Inkloom.Api.Test;

[TestCaseOrderer(ordererTypeName: "Inkloom.Api.Test.TestCaseOrderer", ordererAssemblyName: "Inkloom.Api.Test")]
[Collection("Database Collection")]
// [TestCollectionPriority(2)]
public partial class UserServiceTests(IUserService userService)
{
    public static readonly User testUser = SeedData.Users[0];
}