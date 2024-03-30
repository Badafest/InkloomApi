using InkloomApi.Models;
namespace test;

public static class UserSeed
{
    public static readonly List<User> data = [
        new(){Id = 2, Email = "user@inkloom.com", Password = "Str0ngPassword123", Username = "user" }
    ];
}
