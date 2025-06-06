using Inkloom.Api.Assets;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Inkloom.Api
{
    public static class Constants
    {
        public const string AUTH_SCHEME = JwtBearerDefaults.AuthenticationScheme;
        public const string DEFAULT_ROUTE = "api/v1/[controller]";

        public const string DEFAULT_ERROR_MESSAGE = "Something Went Wrong";
    }
}