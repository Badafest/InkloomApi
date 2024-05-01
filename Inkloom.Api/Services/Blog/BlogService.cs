
using System.Net;

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
        await HandleBlogTags(blog, blog.Tags.GetRange(0, blog.Tags.Count));
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
        var blog = await _context.Blogs.Where(blog => blog.Id == blogId).Include(blog => blog.Tags).FirstOrDefaultAsync();
        if (blog?.Id != blogId)
        {
            return new(HttpStatusCode.NotFound) { Message = "Blog not found" };
        }
        return new() { Data = _mapper.Map<BlogResponse>(blog) };
    }

    public async Task<ServiceResponse<BlogPreviewResponse>> GetBlogPreview(int blogId)
    {
        var blog = await _context.Blogs.Where(blog => blog.Id == blogId).Include(blog => blog.Tags).FirstOrDefaultAsync();
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
            blog.Title.ToLower().Contains(searchData.SearchText.ToLower()) ||
            blog.Description == null ||
            blog.Description.ToLower().Contains(searchData.SearchText.ToLower()))
        .Include(blog => blog.Tags)
        .Where(blog => searchData.Tags == null ||
            searchData.Tags.All(searchName => blog.Tags.Any(tag => tag.Name == searchName.ToUpper())))
        .Skip((searchData.Page - 1) * 100)
        .Take(100)
        .Select(blog => _mapper.Map<BlogResponse>(blog))
        .ToArrayAsync();

        return new() { Data = blogs };
    }

    public async Task<ServiceResponse<BlogResponse>> UpdateBlog(int Id, BlogRequest updateData)
    {
        var blog = await _context.Blogs.Where(blog => blog.Id == Id).Include(blog => blog.Tags).FirstOrDefaultAsync();
        if (blog?.Id != Id)
        {
            return new(HttpStatusCode.NotFound) { Message = "Blog not found" };
        }
        var updateBlog = _mapper.Map<Blog>(updateData);

        blog.Public = updateData.Public ?? blog.Public;
        blog.Status = updateData.Status ?? blog.Status;
        blog.Title = updateBlog.Title ?? blog.Title;
        blog.Description = updateBlog.Description ?? blog.Description;
        blog.HeaderImage = updateBlog.HeaderImage ?? blog.HeaderImage;
        blog.Content = updateBlog.Content ?? blog.Content;

        if (updateData.Tags != null)
        {
            await HandleBlogTags(blog, updateBlog.Tags);
        }
        await _context.SoftSaveChangesAsync();

        return new() { Data = _mapper.Map<BlogResponse>(blog) };
    }

    private async Task HandleBlogTags(Blog blog, IEnumerable<Tag> updatedTags)
    {
        var oldTagNames = blog.Tags.Select(tag => tag.Name.ToUpper());
        blog.Tags.RemoveAll(tag => true);
        await _context.SaveChangesAsync();

        var updatedTagNames = updatedTags.Select(tag => tag.Name.ToUpper());

        var foundTags = await _context.Tags.Where(tag => updatedTagNames.Contains(tag.Name.ToUpper())).ToArrayAsync();
        blog.Tags.AddRange(foundTags);

        var foundTagNames = foundTags.Select(tag => tag.Name.ToUpper());

        var newTags = updatedTags.Where(tag => !foundTagNames.Contains(tag.Name));
        blog.Tags.AddRange(newTags);
    }
}
