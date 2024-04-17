using System.Net;
using Microsoft.IdentityModel.Tokens;

namespace Inkloom.Api.Middlewares;
public class ExceptionMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ArgumentException exception)
        {
            await Handler(context, exception, HttpStatusCode.BadRequest);
        }
        catch (SecurityTokenException exception)
        {
            await Handler(context, exception, HttpStatusCode.BadRequest);
        }
        catch (Exception exception)
        {
            await Handler(context, exception);
        }
    }

    private static async Task Handler(HttpContext context, Exception error, HttpStatusCode code = HttpStatusCode.InternalServerError)
    {
        var message = error.Message ?? DEFAULT_ERROR_MESSAGE;
        var response = new ServiceResponse<string>(code) { Message = message };
        context.Response.StatusCode = (int)response.Status;
        await context.Response.WriteAsJsonAsync(response);
    }
}

public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionMiddleware(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionMiddleware>();
    }
};