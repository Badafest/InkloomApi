using Inkloom.Api.Data;
using Inkloom.Api.Data.Models;
using Inkloom.Api.Services;

namespace Inkloom.Api.Test;

[Collection("Database Collection")]
public partial class AuthServiceTests(IAuthService authService, ITokenService tokenService, DataContext dataContext)
{
    public static readonly User testUser = SeedData.Users[0];
    private readonly string testUserPassword = "Str0ngPassword123";
}