using Inkloom.Api.Models;
namespace Inkloom.Api.Test;

public static class UserSeed
{
    public static readonly List<User> data = [
        new(){Email = "user@inkloom.com", Password = "Str0ngPassword123", Username = "user" }
    ];
}
