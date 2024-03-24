namespace InkloomApi.Controllers;
[ApiController]
[Route(DEFAULT_ROUTE)]
public class PingController : ControllerBase
{
    [HttpGet]
    public string Test()
    {
        return "PONG";
    }
}
