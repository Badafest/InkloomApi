using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace InkloomApi
{
    public static class Constants
    {

        public const string AUTH_SCHEME = JwtBearerDefaults.AuthenticationScheme;
        public const string DEFAULT_ROUTE = "api/v1/[controller]/[action]";

        public const string DEFAULT_ERROR_MESSAGE = "Something Went Wrong";
    }
}