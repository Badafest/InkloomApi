using Inkloom.Api.Assets;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;

namespace Inkloom.Api.Controllers;

[ApiController]
[Route(DEFAULT_ROUTE)]
[Authorize]
[Authorize("EmailVerified")]
public class BlogController(IBlogService blogService, IAssetManager assetManager, IConfiguration configuration) : ControllerBase
{
    private readonly IBlogService _blogService = blogService;
    private readonly IAssetManager _assetManager = assetManager;
    private readonly IConfiguration _configuration = configuration;

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
        var serviceResponse = await _blogService.GetBlogById(Id, HttpContext.User?.Identity?.Name ?? "");
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
    public async Task<ActionResult<ServiceResponse<BlogResponse>>> CreateBlog([FromForm] BlogRequest newBlog, IFormFileCollection? images)
    {
        if (images?.Count > 0)
        {
            HandleBlogImages(newBlog);
        }
        var serviceResponse = await _blogService.CreateBlog(newBlog, User?.Identity?.Name ?? "");
        return StatusCode((int)serviceResponse.Status, serviceResponse);
    }

    [HttpPatch("{Id}")]
    public async Task<ActionResult<ServiceResponse<BlogResponse>>> UpdateBlog(int Id, [FromForm] BlogRequest updateData, IFormFileCollection? images)
    {
        if (images?.Count > 0)
        {
            HandleBlogImages(updateData);
        }
        var serviceResponse = await _blogService.UpdateBlog(Id, updateData, HttpContext.User?.Identity?.Name ?? "");
        return StatusCode((int)serviceResponse.Status, serviceResponse);
    }

    [HttpDelete("{Id}")]
    public async Task<ActionResult<ServiceResponse<BlogResponse>>> DeleteBlog(int Id)
    {
        var serviceResponse = await _blogService.DeleteBlog(Id, HttpContext.User?.Identity?.Name ?? "");
        return StatusCode((int)serviceResponse.Status, serviceResponse);
    }

    private void HandleBlogImages(BlogRequest blogRequest)
    {
        var assets = (List<Asset>)HttpContext.Items["Assets"]!;
        foreach (var asset in assets)
        {
            var splitName = asset.Record.Name.Split("_");
            var uuid = splitName[0];
            asset.Record.Name = string.Join("_", splitName.Skip(1));
            var isHeaderImage = uuid == "headerImage";

            var blockImage = BlogHelper.ParseBlogContent(blogRequest.Content ?? []).
                FirstOrDefault(block => block?.Metadata?["uuid"] == uuid);

            if (!isHeaderImage && blockImage == null)
            {
                continue;
            }

            var assetId = _assetManager.AddAsset(asset.Record.FilePath, asset);
            var assetLink = $"{_configuration["ApiBaseUrl"]}/asset/{assetId}{Path.GetExtension(asset.Record.Name)}";

            if (isHeaderImage)
            {
                blogRequest.HeaderImage = assetLink;
            }
            else
            {
                blockImage!.Content = assetLink;
            }
        }
    }
}