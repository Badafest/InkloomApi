namespace InkloomApi.Profiles;
public class BlogProfile : Profile
{
    public BlogProfile()
    {
        CreateMap<Blog, BlogResponse>();
        CreateMap<CreateBlogRequest, Blog>();
    }
}