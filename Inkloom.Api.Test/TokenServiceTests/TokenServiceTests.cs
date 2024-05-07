using Inkloom.Api.Data.Models;
using Inkloom.Api.Services;

namespace Inkloom.Api.Test;

[Collection("Database Collection")]
public partial class TokenServiceTests(ITokenService tokenService)
{
    public static readonly User testUser = SeedData.Users[0];
}