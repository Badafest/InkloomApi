
using System.Net;
using System.Text.RegularExpressions;

namespace Inkloom.Api.Services;

public class BlogService(DataContext context, IMapper mapper) : IBlogService
{
    private readonly DataContext _context = context;

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

        await _context.AddAsync(blog);
        await _context.SoftSaveChangesAsync();

        return new() { Data = _mapper.Map<BlogResponse>(blog) };
    }
    public async Task<ServiceResponse<BlogResponse>> DeleteBlog(int blogId)
    {
        var blog = await _context.Blogs.FirstOrDefaultAsync(blog => blog.Id == blogId);
        if (blog?.Id != blogId)
        {
            return new(HttpStatusCode.NotFound) { Message = "Not Found" };
        }
        _context.Remove(blog);
        await _context.SoftSaveChangesAsync();

        return new() { Data = _mapper.Map<BlogResponse>(blog) };
    }

    public async Task<ServiceResponse<BlogResponse>> GetBlogById(int blogId)
    {
        var blog = await _context.Blogs.FirstOrDefaultAsync(blog => blog.Id == blogId);
        if (blog?.Id != blogId)
        {
            return new(HttpStatusCode.NotFound) { Message = "Blog not found" };
        }
        return new() { Data = _mapper.Map<BlogResponse>(blog) };
    }

    public async Task<ServiceResponse<BlogPreviewResponse>> GetBlogPreview(int blogId)
    {
        var blog = await _context.Blogs.FirstOrDefaultAsync(blog => blog.Id == blogId);
        if (blog?.Id != blogId)
        {
            return new(HttpStatusCode.NotFound) { Message = "Blog not found" };
        }
        return new() { Data = _mapper.Map<BlogPreviewResponse>(blog) };
    }

    public async Task<ServiceResponse<BlogResponse[]>> SearchBlogs(SearchBlogRequest searchData)
    {
        var blogs = await _context.Blogs
        .Where(blog => searchData.Author == null || blog.Author != null && blog.Author.Username == searchData.Author)
        .Where(blog => searchData.Status == null || blog.Status == searchData.Status)
        .Where(blog => searchData.Public == null || blog.Public == searchData.Public)
        .Where(blog => searchData.SearchText == null ||
            Regex.IsMatch(blog.Title, Regex.Escape(searchData.SearchText), RegexOptions.IgnoreCase) ||
            blog.Description == null ||
            Regex.IsMatch(blog.Description, Regex.Escape(searchData.SearchText), RegexOptions.IgnoreCase))
        .Where(blog => searchData.Tags == null ||
            searchData.Tags.All(searchName => blog.Tags.Any(tag => tag.Name.Equals(searchName, StringComparison.CurrentCultureIgnoreCase))))
        .Skip((searchData.Page - 1) * 100)
        .Take(100)
        .Select(blog => _mapper.Map<BlogResponse>(blog))
        .ToArrayAsync();

        return new() { Data = blogs };
    }

    public async Task<ServiceResponse<BlogResponse>> UpdateBlog(int Id, BlogRequest updateData)
    {
        var blog = await _context.Blogs.FirstOrDefaultAsync(blog => blog.Id == Id);
        if (blog?.Id != Id)
        {
            return new(HttpStatusCode.BadRequest) { Message = "Blog not found" };
        }

        blog.Public = updateData.Public ?? blog.Public;
        blog.Status = updateData.Status ?? blog.Status;
        blog.Title = updateData.Title ?? blog.Title;
        blog.Description = updateData.Description ?? blog.Description;
        blog.HeaderImage = updateData.HeaderImage ?? blog.HeaderImage;
        blog.Content = updateData.Content ?? blog.Content;

        await _context.AddAsync(blog);
        await _context.SoftSaveChangesAsync();

        return new() { Data = _mapper.Map<BlogResponse>(blog) };
    }
}
