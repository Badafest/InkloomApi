using System.Net;
using Microsoft.IdentityModel.JsonWebTokens;

namespace InkloomApi.Middlewares;

public class TokenBlacklistMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;


    private readonly ServiceResponse<string?> serviceResponse = new(HttpStatusCode.Forbidden) { Message = "Invalid Token" };

    public async Task InvokeAsync(HttpContext httpContext, DataContext dataContext)
    {
        var username = httpContext.User.Identity?.Name ?? "";
        if (username.Length == 0)
        {
            await _next(httpContext);
            return;
        }
        var user = await dataContext.Users.FirstOrDefaultAsync(user => user.Username == username);
        if (user?.Username != username)
        {
            httpContext.Response.StatusCode = (int)serviceResponse.Status;
            await httpContext.Response.WriteAsJsonAsync(serviceResponse);
            return;
        }

        var parsedIat = long.TryParse(httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Nbf) ?? "", out long tokenIssuedTimestamp);

        var blacklistDatetime = user?.TokenBlacklistTimestamp ?? new();

        DateTime.SpecifyKind(blacklistDatetime, DateTimeKind.Utc);

        var blacklistTimestamp = new DateTimeOffset(blacklistDatetime).ToUnixTimeSeconds();

        if (!parsedIat || (tokenIssuedTimestamp <= blacklistTimestamp))
        {
            httpContext.Response.StatusCode = (int)serviceResponse.Status;
            await httpContext.Response.WriteAsJsonAsync(serviceResponse);
            return;
        }
        await _next(httpContext);
    }
}

public static class TokenBlacklistMiddlewareExtensions
{
    public static IApplicationBuilder UseTokenBlacklistMiddleware(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TokenBlacklistMiddleware>();
    }
}