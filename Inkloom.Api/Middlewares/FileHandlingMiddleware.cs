using System.Net;
using Inkloom.Api.Assets;

namespace Inkloom.Api.Middlewares;
public class FileHandlingMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;
    private readonly long _maxFileSize = 2 * 1024 * 1024; // 2MB
    private readonly Dictionary<AssetType, string[]> _allowedMimeTypes = new()
    {
        {AssetType.Image, ["image/jpg", "image/png", "image/svg+xml", "image/webp", "image/svg", "image/jpeg"]},
        // {AssetType.Audio, ["audio/mpeg", "audio/webm"]},
        // {AssetType.Video, ["video/mp4", "video/webm"]},
        // {AssetType.Document, ["application/pdf", "text/plain"]},
        // {AssetType.File, []},
    };

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.HasFormContentType)
        {
            await _next(context);
            return;
        }

        var form = await context.Request.ReadFormAsync();

        if (form == null || form.Files.Count == 0)
        {
            await _next(context);
            return;
        }

        var assets = new List<Asset>();

        var author = context.User.Identity?.Name;

        if (string.IsNullOrEmpty(author))
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return;
        }

        foreach (var file in form!.Files)
        {
            if (file.Length == 0)
            {
                continue;
            }
            AssetType? assetType = null;
            var fileMimetype = file.ContentType;

            foreach (var allowedType in _allowedMimeTypes)
            {
                if (allowedType.Value.Contains(fileMimetype))
                {
                    assetType = allowedType.Key;
                }
            }

            if (assetType == null)
            {
                var message = $"File of type {fileMimetype} is not supported";
                await TerminateWithResponse(context, HttpStatusCode.UnsupportedMediaType, message);
                return;
            }

            if (file.Length > _maxFileSize)
            {
                var message = $"File ({file.Length}B) is larger than allowed limit ({_maxFileSize}B).";
                await TerminateWithResponse(context, HttpStatusCode.RequestEntityTooLarge, message);
                return;
            }

            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            var asset = new Asset
            {
                Stream = file.OpenReadStream(),
                Record = new AssetRecord
                {
                    Name = file.FileName,
                    FilePath = uniqueFileName,
                    Author = author,
                    Type = assetType ?? AssetType.Document,
                    Mimetype = file.ContentType,
                    Size = file.Length
                }
            };

            assets.Add(asset);
        }

        context.Items["Assets"] = assets;

        await _next(context);
    }

    private static async Task TerminateWithResponse(HttpContext context, HttpStatusCode statusCode, string message)
    {
        var serviceResponse = new ServiceResponse<string>(statusCode)
        {
            Message = message
        };
        context.Response.StatusCode = (int)serviceResponse.Status;
        await context.Response.WriteAsJsonAsync(serviceResponse);
        return;
    }
}


public static class FileHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseFileHandlingMiddleware(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<FileHandlingMiddleware>();
    }
}