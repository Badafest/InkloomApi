namespace InkloomApi.Models
{
    public class User
    {
        public int Id { get; set; } = 0;
        public string Email { get; set; } = "";
        public string Username { get; set; } = "";

        public string Password { get; set; } = "";
    }

}