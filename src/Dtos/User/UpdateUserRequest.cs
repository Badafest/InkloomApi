namespace InkloomApi.Dtos
{
    public class UpdateUserRequest
    
    {
        public string Name { get; set; } = "Anonymous User";
        public string Password { get; set; } = string.Empty;
    }

}