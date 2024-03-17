using InkloomApi.Services;

namespace test;

[TestCaseOrderer(ordererTypeName: "test.TestCaseOrderer", ordererAssemblyName: "test")]
[Collection("AuthServiceTests")]
[TestCollectionPriority(1)]
public partial class AuthServiceTests
{
    private static readonly Configuration configuration = new();
    private static readonly AuthService authService = new(Configuration.config, configuration.dataContext, configuration.autoMapper);

}