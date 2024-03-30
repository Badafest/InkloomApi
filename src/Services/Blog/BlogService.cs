
namespace InkloomApi.Services;

public class BlogService : IBlogService
{
    public Task<ServiceResponse<BlogResponse>> CreateBlog(CreateBlogRequest newBlog)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse<BlogResponse>> DeleteBlog(int blogId)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse<BlogResponse>> GetBlogById(int blogId)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse<BlogResponse>> GetBlogPreview(int blogId)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse<BlogResponse[]>> GetMyBlogs(SearchOwnBlogRequest searchData)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse<BlogResponse[]>> GetPublicBlogs(SearchPublicBlogRequest searchData)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse<BlogResponse>> UpdateBlog(int Id, UpdateBlogRequest updateData)
    {
        throw new NotImplementedException();
    }
}
