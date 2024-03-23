using InkloomApi.Models;
namespace test;

public static class UserSeed
{
    public static readonly IEnumerable<User> data = [
        new(){ Email = "user@inkloom.com", Password = "Str0ngPassword123", Username = "user" }
    ];
}
