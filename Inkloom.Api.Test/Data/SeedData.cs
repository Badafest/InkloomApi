using Inkloom.Api.Data.Models;
namespace Inkloom.Api.Test;

public static class SeedData
{
    public static readonly User[] Users = [
        new(){Email = "user@inkloom.com", Password = "Str0ngPassword123", Username = "user", Id=1 }
    ];
}
