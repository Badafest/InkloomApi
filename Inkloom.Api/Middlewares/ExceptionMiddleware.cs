using System.Net;
using Microsoft.IdentityModel.Tokens;

namespace Inkloom.Api.Middlewares;
public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ArgumentException exception)
        {
            _logger.LogError(exception: exception, "Argument Exception");
            await Handler(context, exception, HttpStatusCode.BadRequest);
        }
        catch (SecurityTokenException exception)
        {
            _logger.LogError(exception: exception, "Security Token Exception");
            await Handler(context, exception, HttpStatusCode.BadRequest);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception: exception, "Generic Exception");
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