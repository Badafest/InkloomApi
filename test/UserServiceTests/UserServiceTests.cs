using InkloomApi.Services;

namespace test;

[TestCaseOrderer(ordererTypeName: "test.TestCaseOrderer", ordererAssemblyName: "test")]
[Collection("UserServiceTests")]
[TestCollectionPriority(2)]
public partial class UserServiceTests
{
    private static readonly Configuration configuration = new();

    public static readonly UserService userService = new(configuration.autoMapper, configuration.dataContext);

}