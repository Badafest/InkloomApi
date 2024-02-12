using System.Text.Json.Serialization;

namespace InkloomApi.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public class User
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; } = "Anonymous User";
        public string Username { get; set; } = "anonymous";

        public string Password { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;

        public DateTime RefreshTokenExpiry { get; set; }
    }
}