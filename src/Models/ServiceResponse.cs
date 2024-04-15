using System.Net;

namespace InkloomApi.Models;
public class ServiceResponse<T>(HttpStatusCode status = HttpStatusCode.OK)
{
    public T? Data { get; set; }
    public string? Message { get; set; }

    public bool Success { get; } = (int)status < 400;

    public HttpStatusCode Status = status;
}
