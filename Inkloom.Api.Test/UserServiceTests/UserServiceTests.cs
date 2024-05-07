using Inkloom.Api.Data.Models;
using Inkloom.Api.Services;

namespace Inkloom.Api.Test;

[Collection("Database Collection")]
public partial class UserServiceTests(IUserService userService)
{
    public static readonly User testUser = SeedData.Users[0];
}