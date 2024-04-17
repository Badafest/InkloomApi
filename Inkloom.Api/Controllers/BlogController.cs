using Microsoft.AspNetCore.Authorization;

namespace Inkloom.Api.Controllers;

[ApiController]
[Route(DEFAULT_ROUTE)]
[Authorize]
[Authorize("EmailVerified")]
public class BlogController(IBlogService blogService) : ControllerBase
{

    private readonly IBlogService _blogService = blogService;

    [HttpGet("Public")]
    public async Task<ActionResult<ServiceResponse<BlogResponse[]>>> GetPublicBlogs([FromQuery] SearchPublicBlogRequest publicSearchData)
    {
        var searchData = new SearchBlogRequest(publicSearchData);
        var serviceResponse = await _blogService.SearchBlogs(searchData);
        return StatusCode((int)serviceResponse.Status, serviceResponse);
    }

    [HttpGet]
    public async Task<ActionResult<ServiceResponse<BlogResponse[]>>> GetMyBlogs([FromQuery] SearchOwnBlogRequest ownSearchData)
    {
        var searchData = new SearchBlogRequest(ownSearchData) { Author = User?.Identity?.Name ?? "" };
        var serviceResponse = await _blogService.SearchBlogs(searchData);
        return StatusCode((int)serviceResponse.Status, serviceResponse);
    }

    [HttpGet("{Id}")]
    public async Task<ActionResult<ServiceResponse<BlogResponse>>> GetBlogById(int Id)
    {
        var serviceResponse = await _blogService.GetBlogById(Id);
        return StatusCode((int)serviceResponse.Status, serviceResponse);
    }

    [HttpGet("{Id}/Preview")]
    [AllowAnonymous]
    public async Task<ActionResult<ServiceResponse<BlogPreviewResponse>>> GetBlogPreview(int Id)
    {
        var serviceResponse = await _blogService.GetBlogPreview(Id);
        return StatusCode((int)serviceResponse.Status, serviceResponse);
    }

    [HttpPost]
    public async Task<ActionResult<ServiceResponse<BlogResponse>>> CreateBlog(CreateBlogRequest newBlog)
    {
        var serviceResponse = await _blogService.CreateBlog(newBlog, User?.Identity?.Name ?? "");
        return StatusCode((int)serviceResponse.Status, serviceResponse);
    }

    [HttpPatch("{Id}")]
    public async Task<ActionResult<ServiceResponse<BlogResponse>>> UpdateBlog(UpdateBlogRequest updateData, int Id)
    {
        var serviceResponse = await _blogService.UpdateBlog(Id, updateData);
        return StatusCode((int)serviceResponse.Status, serviceResponse);
    }

    [HttpDelete("{Id}")]
    public async Task<ActionResult<ServiceResponse<BlogResponse>>> DeleteBlog(int Id)
    {
        var serviceResponse = await _blogService.DeleteBlog(Id);
        return StatusCode((int)serviceResponse.Status, serviceResponse);
    }
}