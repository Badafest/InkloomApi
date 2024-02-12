namespace InkloomApi.Data
{
    public static class UserData
    {
        private static readonly List<User> Users = new()
        {
            new(){Id=0, Username="admin", Password="iamadmin"},
            new(){Id=1, Username="frodo", Password="iamfrodo"},
            new(){Id=2, Username="michael", Password="12345678"},
            new(){Id=3, Username="john", Password="john@123"},
        };

        public static User? FindUser(string username)
        {
            var user = Users.FirstOrDefault(user => user.Username == username);
            return user?.Username == username ? user : null;
        }

        public static User? UpdateUser(User updatedUser)
        {
            var user = FindUser(updatedUser.Username);
            if (user == null)
            {
                return user;
            }
            if (updatedUser.Password != null)
            {
                user.Password = updatedUser.Password;
            }
            if (updatedUser.RefreshToken != null)
            {
                user.RefreshToken = updatedUser.RefreshToken;
            }
            if (updatedUser.RefreshTokenExpiry > DateTime.Now)
            {
                user.RefreshTokenExpiry = updatedUser.RefreshTokenExpiry;
            }
            return user;
        }
    }
}