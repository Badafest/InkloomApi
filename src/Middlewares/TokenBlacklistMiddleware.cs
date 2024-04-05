using System.Net;
using Microsoft.IdentityModel.JsonWebTokens;

namespace InkloomApi.Middlewares;

public class TokenBlacklistMiddleware(RequestDelegate next, DataContext dataContext)
{
    private readonly RequestDelegate _next = next;

    private readonly DataContext _dataContext = dataContext;

    private readonly ServiceResponse<string?> serviceResponse = new(HttpStatusCode.Forbidden) { Message = "Invalid Token" };

    public async Task InvokeAsync(HttpContext httpContext)
    {
        var username = httpContext.User.Identity?.Name ?? "";
        if (username.Length == 0)
        {
            await _next(httpContext);
            return;
        }
        var user = await _dataContext.Users.FirstOrDefaultAsync(user => user.Username == username);
        if (user?.Username != username)
        {
            httpContext.Response.StatusCode = (int)serviceResponse.Status;
            await httpContext.Response.WriteAsJsonAsync(serviceResponse);
        }
        var iat = JwtRegisteredClaimNames.Iat;
        Console.WriteLine($"Token Issued At: {httpContext.User.FindFirstValue(iat)}");
        if (user?.TokenBlacklistTimestamp > DateTime.Parse(httpContext.User.FindFirstValue(iat) ?? ""))
        {
            httpContext.Response.StatusCode = (int)serviceResponse.Status;
            await httpContext.Response.WriteAsJsonAsync(serviceResponse);
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