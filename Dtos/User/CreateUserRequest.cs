namespace InkloomApi.Dtos
{
    public class CreateUserRequest
    {
        public string Name { get; set; } = "Anonymous User";
        public string Username { get; set; } = "anonymous";

        public string Password { get; set; } = string.Empty;
    }

}