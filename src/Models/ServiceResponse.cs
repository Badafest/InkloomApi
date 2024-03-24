using System.Net;

namespace InkloomApi.Models;
public class ServiceResponse<T>
{
    public T? Data { get; set; }
    public string? Message { get; set; }

    public bool Success { get; } = true;

    public HttpStatusCode Status = HttpStatusCode.OK;


    public ServiceResponse(HttpStatusCode status = HttpStatusCode.OK)
    {
        Success = (int)status < 400;
        Status = status;
    }
}
