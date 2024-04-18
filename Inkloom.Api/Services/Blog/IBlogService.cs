namespace Inkloom.Api.Services;

public interface IBlogService
{
    public Task<ServiceResponse<BlogResponse>> CreateBlog(BlogRequest newBlog, string authorUsername);
    public Task<ServiceResponse<BlogResponse>> UpdateBlog(int Id, BlogRequest updateData);
    public Task<ServiceResponse<BlogResponse>> DeleteBlog(int blogId);
    public Task<ServiceResponse<BlogResponse[]>> SearchBlogs(SearchBlogRequest searchData);

    public Task<ServiceResponse<BlogResponse>> GetBlogById(int blogId);
    public Task<ServiceResponse<BlogPreviewResponse>> GetBlogPreview(int blogId);
}