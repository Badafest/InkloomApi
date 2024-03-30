using InkloomApi.Models;
using InkloomApi.Services;

namespace test;

[TestCaseOrderer(ordererTypeName: "test.TestCaseOrderer", ordererAssemblyName: "test")]
[Collection("Database Collection")]
// [TestCollectionPriority(2)]
public partial class UserServiceTests(Configuration configuration)
{
    private readonly UserService userService = new(configuration.autoMapper, configuration.dataContext);
    public static readonly User testUser = UserSeed.data[0];
}