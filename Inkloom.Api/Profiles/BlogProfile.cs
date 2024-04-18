namespace Inkloom.Api.Profiles;

public class BlogTagsResolver : IValueResolver<Blog, BlogPreviewResponse, IEnumerable<string>>
{
    public IEnumerable<string> Resolve(Blog source, BlogPreviewResponse destination, IEnumerable<string> destinatinoMember, ResolutionContext context)
    {
        return source.Tags.Select(tag => tag.Name);
    }
}


public class BlogRequestTagsResolver : IValueResolver<BlogRequest, Blog, IEnumerable<Tag>>
{
    public IEnumerable<Tag> Resolve(BlogRequest source, Blog destination, IEnumerable<Tag> destinatinoMember, ResolutionContext context)
    {
        return source.Tags?.Select(tag => new Tag() { Name = tag }) ?? [];
    }
}
public class BlogProfile : Profile
{
    public BlogProfile()
    {
        CreateMap<Blog, BlogPreviewResponse>().ForMember(dest => dest.Tags, opt => opt.MapFrom(new BlogTagsResolver()));
        CreateMap<Blog, BlogResponse>().ForMember(dest => dest.Tags, opt => opt.MapFrom(new BlogTagsResolver()));
        CreateMap<BlogRequest, Blog>().ForMember(dest => dest.Tags, opt => opt.MapFrom(new BlogRequestTagsResolver()));
    }
}