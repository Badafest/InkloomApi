namespace Inkloom.Api.Controllers;

[ApiController]
[Route(DEFAULT_ROUTE)]

public class TagController(ITagService tagService) : ControllerBase
{

    private readonly ITagService _tagService = tagService;

    [HttpGet]
    public async Task<ActionResult<ServiceResponse<IEnumerable<string>>>> GetTags(string name)
    {
        var serviceResponse = await _tagService.GetTags(name);
        return StatusCode((int)serviceResponse.Status, serviceResponse);
    }
}