using Microsoft.AspNetCore.Authorization;

namespace InkloomApi.Controllers;

[ApiController]
[Route(DEFAULT_ROUTE)]
[Authorize]
[Authorize("EmailVerified")]
public class BlogController(IBlogService blogService) : ControllerBase
{

    private readonly IBlogService _blogService = blogService;

    [HttpGet("Public")]
    public async Task<ActionResult<ServiceResponse<BlogResponse[]>>> GetPublicBlogs(SearchPublicBlogRequest searchData)
    {
        var serviceResponse = await _blogService.GetPublicBlogs(searchData);
        return StatusCode((int)serviceResponse.Status, serviceResponse);
    }

    [HttpGet]
    public async Task<ActionResult<ServiceResponse<BlogResponse[]>>> GetMyBlogs(SearchOwnBlogRequest searchData)
    {
        var serviceResponse = await _blogService.GetMyBlogs(searchData);
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
    public async Task<ActionResult<ServiceResponse<BlogResponse>>> GetBlogPreview(int Id)
    {
        var serviceResponse = await _blogService.GetBlogPreview(Id);
        return StatusCode((int)serviceResponse.Status, serviceResponse);
    }

    [HttpPost]
    public async Task<ActionResult<ServiceResponse<BlogResponse>>> CreateBlog(CreateBlogRequest newBlog)
    {
        var serviceResponse = await _blogService.CreateBlog(newBlog);
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