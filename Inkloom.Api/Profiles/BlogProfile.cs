namespace Inkloom.Api.Profiles;

public class BlogTagsResolver : IValueResolver<Blog, BlogPreviewResponse, IEnumerable<string>>
{
    public IEnumerable<string> Resolve(Blog source, BlogPreviewResponse destination, IEnumerable<string> destinatinoMember, ResolutionContext context)
    {
        return source.Tags.Select(tag => tag.Name);
    }
}


public class BlogRequestTagsResolver : IValueResolver<BlogRequest, Blog, List<Tag>>
{
    public List<Tag> Resolve(BlogRequest source, Blog destination, List<Tag> destinatinoMember, ResolutionContext context)
    {
        return source.Tags?.Select(tag => new Tag() { Name = tag.ToUpper() })?.ToList() ?? [];
    }
}

public class BlogAuthorResolver : IValueResolver<Blog, BlogPreviewResponse, AuthorResponse?>
{
    public AuthorResponse? Resolve(Blog source, BlogPreviewResponse destination, AuthorResponse? destinatinoMember, ResolutionContext context)
    {
        if (source.Author == null) { return null; }
        return new() { Username = source.Author.Username, About = source.Author.About, Avatar = source.Author.Avatar };
    }
}
public class BlogProfile : Profile
{
    public BlogProfile()
    {
        CreateMap<Blog, BlogPreviewResponse>().
            ForMember(dest => dest.Tags, opt => opt.MapFrom(new BlogTagsResolver())).
            ForMember(dest => dest.Author, opt => opt.MapFrom(new BlogAuthorResolver()));
        CreateMap<Blog, BlogResponse>().
            ForMember(dest => dest.Tags, opt => opt.MapFrom(new BlogTagsResolver())).
            ForMember(dest => dest.Author, opt => opt.MapFrom(new BlogAuthorResolver()));
        CreateMap<BlogRequest, Blog>().
            ForMember(dest => dest.Tags, opt => opt.MapFrom(new BlogRequestTagsResolver()));
    }
}