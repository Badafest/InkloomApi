using Microsoft.AspNetCore.Authorization;

namespace InkloomApi.Controllers
{
    [ApiController]
    [Route(DEFAULT_ROUTE)]
    [Authorize]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public ServiceResponse<object> Secure()
        {
            return new() { Message = "Allowed" };
        }
    }

}