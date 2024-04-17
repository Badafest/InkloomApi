namespace Inkloom.Api.Controllers;
[ApiController]
[Route(DEFAULT_ROUTE)]
public class PingController : ControllerBase
{
    [HttpGet]
    public string Index()
    {
        return "PONG";
    }
}
