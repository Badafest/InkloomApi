using System.Text.Json;

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
        return source.Tags?.Select(tag => new Tag() { Name = tag.ToLower() })?.ToList() ?? [];
    }
}

public class BlogAuthorResolver : IValueResolver<Blog, BlogPreviewResponse, AuthorResponse?>
{
    public AuthorResponse? Resolve(Blog source, BlogPreviewResponse destination, AuthorResponse? destinatinoMember, ResolutionContext context)
    {
        if (source.Author == null) { return null; }
        return new()
        {
            Username = source.Author.Username,
            DisplayName = source.Author.DisplayName,
            About = source.Author.About,
            Avatar = source.Author.Avatar
        };
    }
}

public class BlogContentResolver : IValueResolver<Blog, BlogResponse, IEnumerable<ContentBlock>>
{
    public IEnumerable<ContentBlock> Resolve(Blog source, BlogResponse destination, IEnumerable<ContentBlock> destinatinoMember, ResolutionContext context)
    {
        if (string.IsNullOrWhiteSpace(source.Content?.Trim()))
        {
            return [];
        }
        return BlogHelper.ParseBlogContent(source.Content);
    }
}

public class BlogRequestContentResolver : IValueResolver<BlogRequest, Blog, string?>
{
    public string? Resolve(BlogRequest source, Blog destination, string? destinatinoMember, ResolutionContext context)
    {
        var parsedBlocks = BlogHelper.ParseBlogContent(source.Content ?? []);
        if (!parsedBlocks.Any())
        {
            return null;
        }
        return JsonSerializer.Serialize(parsedBlocks);
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
            ForMember(dest => dest.Author, opt => opt.MapFrom(new BlogAuthorResolver())).
            ForMember(dest => dest.Content, opt => opt.MapFrom(new BlogContentResolver()));
        CreateMap<BlogRequest, Blog>().
            ForMember(dest => dest.Tags, opt => opt.MapFrom(new BlogRequestTagsResolver())).
            ForMember(dest => dest.Content, opt => opt.MapFrom(new BlogRequestContentResolver()));
    }
}