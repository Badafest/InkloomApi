
using System.Net;
using Inkloom.Api.Assets;
using Inkloom.Api.Extensions;

namespace Inkloom.Api.Services;

public class BlogService(DataContext context, IMapper mapper, IConfiguration configuration, IAssetManager assetManager) : IBlogService
{
    private readonly DataContext _context = context;
    private readonly IConfiguration _configuration = configuration;
    private readonly IAssetManager _assetManager = assetManager;
    private readonly IMapper _mapper = mapper;
    public async Task<ServiceResponse<BlogResponse>> CreateBlog(BlogRequest newBlog, string authorUsername)
    {
        var author = await _context.Users.FirstOrDefaultAsync(user => user.Username == authorUsername);
        if (author?.Username != authorUsername)
        {
            return new(HttpStatusCode.BadRequest) { Message = "Author not found" };
        }
        var blog = _mapper.Map<Blog>(newBlog);
        blog.Author = author;
        await HandleBlogTags(blog, blog.Tags.GetRange(0, blog.Tags.Count));
        await _context.AddAsync(blog);
        await _context.SoftSaveChangesAsync();

        return new() { Data = _mapper.Map<BlogResponse>(blog) };
    }
    public async Task<ServiceResponse<BlogResponse>> GetBlogById(int blogId, string currentUsername)
    {
        var blog = await _context.Blogs
            .Where(blog => blog.Id == blogId)
            .Include(blog => blog.Tags)
            .Include(blog => blog.Author)
            .FirstOrDefaultAsync();

        if (blog?.Id != blogId)
        {
            return new(HttpStatusCode.NotFound) { Message = "Blog not found" };
        }

        var currentUser = await _context.Users.FirstOrDefaultAsync(user => user.Username == currentUsername);

        if (!blog.Public && blog.Author != null && blog.AuthorId != currentUser?.Id && !blog.Author.Followers.Any(follower => follower.FollowerId == currentUser?.Id))
        {
            return new(HttpStatusCode.Forbidden) { Message = $"You need to follow {blog.Author.DisplayName} to read this blog" };
        }
        return new() { Data = _mapper.Map<BlogResponse>(blog) };
    }
    public async Task<ServiceResponse<BlogResponse>> UpdateBlog(int Id, BlogRequest updateData, string currentUsername)
    {
        var blog = await _context.Blogs
            .Where(blog => blog.Id == Id)
            .Include(blog => blog.Tags)
            .Include(blog => blog.Author)
            .FirstOrDefaultAsync();

        if (blog?.Id != Id)
        {
            return new(HttpStatusCode.NotFound) { Message = "Blog not found" };
        }

        if (blog.Author!.Username != currentUsername)
        {
            return new(HttpStatusCode.Forbidden) { Message = "Access Denied to Private Blog" };
        }
        var updateBlog = _mapper.Map<Blog>(updateData);

        // delete old images
        _assetManager.DeleteOldFile(_configuration["ApiBaseUrl"]!, blog.HeaderImage ?? "", updateData.HeaderImage ?? "");
        foreach (var block in BlogHelper.ParseBlogContent(blog.Content ?? "[]").Where(block => block.Type == ContentBlockType.image))
        {
            _assetManager.DeleteOldFile(_configuration["ApiBaseUrl"]!, block.Content ?? "");
        }

        if (updateBlog.Status == BlogStatus.PUBLISHED && (blog.Status != BlogStatus.PUBLISHED || blog.PublishedDate <= DateTime.MinValue))
        {
            blog.PublishedDate = DateTime.UtcNow;
        }

        blog.Public = updateData.Public ?? false;
        blog.Status = updateData.Status;
        blog.Title = updateBlog.Title;
        blog.Subtitle = updateBlog.Subtitle;
        blog.HeaderImage = updateBlog.HeaderImage;
        blog.Content = updateBlog.Content;

        await HandleBlogTags(blog, updateBlog.Tags);

        await _context.SoftSaveChangesAsync();

        return new() { Data = _mapper.Map<BlogResponse>(blog) };
    }
    public async Task<ServiceResponse<BlogResponse>> DeleteBlog(int blogId, string currentUsername)
    {
        var blog = await _context.Blogs
            .Where(blog => blog.Id == blogId)
            .Include(blog => blog.Author)
            .FirstOrDefaultAsync();

        if (blog?.Id != blogId)
        {
            return new(HttpStatusCode.NotFound) { Message = "Not Found" };
        }

        if (blog.Author!.Username != currentUsername)
        {
            return new(HttpStatusCode.Forbidden) { Message = "Access Denied to Private Blog" };
        }

        _context.Remove(blog);
        await _context.SoftSaveChangesAsync();

        return new() { Data = _mapper.Map<BlogResponse>(blog) };
    }
    public async Task<ServiceResponse<BlogPreviewResponse>> GetBlogPreview(int blogId)
    {
        var blog = await _context.Blogs
            .Where(blog => blog.Id == blogId)
            .Include(blog => blog.Tags)
            .Include(blog => blog.Author)
            .FirstOrDefaultAsync();

        if (blog?.Id != blogId)
        {
            return new(HttpStatusCode.NotFound) { Message = "Blog not found" };
        }
        return new() { Data = _mapper.Map<BlogPreviewResponse>(blog) };
    }
    public async Task<ServiceResponse<BlogResponse[]>> SearchBlogs(SearchBlogRequest searchData, string currentUsername = "")
    {
        var currentUser = string.IsNullOrEmpty(currentUsername) ? null :
            await _context.Users
                .Include(user => user.Followings)
                .FirstOrDefaultAsync(user => user.Username == currentUsername);

        var blogs = await _context.Blogs
            .Where(blog => searchData.Status == null || blog.Status == searchData.Status)
            .Where(blog => searchData.Public == null || blog.Public == searchData.Public)
            .Where(blog => currentUser == null || currentUser.Followings.Any(following => following.FollowingId == blog.AuthorId))
            .Where(blog => searchData.SearchText == null ||
                blog.Title.Contains(searchData.SearchText) ||
                blog.Subtitle == null ||
                blog.Subtitle.Contains(searchData.SearchText))
            .Include(blog => blog.Author)
            .Where(blog => searchData.Author == null || blog.Author != null && blog.Author.Username == searchData.Author)
            .Include(blog => blog.Tags)
            .Where(blog => searchData.Tags == null ||
                searchData.Tags.All(searchName => blog.Tags.Any(tag => tag.Name == searchName.ToLower())))
            .Skip((searchData.Page - 1) * 100)
            .Take(100)
            .Select(blog => _mapper.Map<BlogResponse>(blog))
            .ToArrayAsync();

        return new() { Data = blogs };
    }
    private async Task HandleBlogTags(Blog blog, IEnumerable<Tag> updatedTags)
    {
        var oldTagNames = blog.Tags.Select(tag => tag.Name.ToLower());
        blog.Tags.RemoveAll(tag => true);
        await _context.SaveChangesAsync();

        var updatedTagNames = updatedTags.Select(tag => tag.Name.ToLower());

        var foundTags = await _context.Tags.Where(tag => updatedTagNames.Contains(tag.Name.ToLower())).ToArrayAsync();
        blog.Tags.AddRange(foundTags);

        var foundTagNames = foundTags.Select(tag => tag.Name.ToLower());

        var newTags = updatedTags.Where(tag => !foundTagNames.Contains(tag.Name));
        blog.Tags.AddRange(newTags);
    }
}
