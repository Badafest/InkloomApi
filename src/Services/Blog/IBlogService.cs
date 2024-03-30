namespace InkloomApi.Services;

public interface IBlogService
{
    public Task<ServiceResponse<BlogResponse>> CreateBlog(CreateBlogRequest newBlog);
    public Task<ServiceResponse<BlogResponse>> UpdateBlog(int Id, UpdateBlogRequest updateData);
    public Task<ServiceResponse<BlogResponse>> DeleteBlog(int blogId);
    public Task<ServiceResponse<BlogResponse[]>> GetMyBlogs(SearchOwnBlogRequest searchData);
    public Task<ServiceResponse<BlogResponse[]>> GetPublicBlogs(SearchPublicBlogRequest searchData);

    public Task<ServiceResponse<BlogResponse>> GetBlogById(int blogId);
    public Task<ServiceResponse<BlogResponse>> GetBlogPreview(int blogId);
}