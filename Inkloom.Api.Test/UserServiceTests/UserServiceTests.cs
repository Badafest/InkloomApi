using Inkloom.Api.Models;
using Inkloom.Api.Services;

namespace Inkloom.Api.Test;

[TestCaseOrderer(ordererTypeName: "Inkloom.Api.Test.TestCaseOrderer", ordererAssemblyName: "Inkloom.Api.Test")]
[Collection("Database Collection")]
// [TestCollectionPriority(2)]
public partial class UserServiceInkloom.Api.Test(Configuration configuration)
{
    private readonly UserService userService = new(configuration.autoMapper, configuration.dataContext);
public static readonly User testUser = UserSeed.data[0];
}