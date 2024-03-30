namespace InkloomApi.Models;

public class Tag : ModelBase
{
    public List<BlogTag> BlogTags { get; } = [];
    public List<Blog> Blogs { get; } = [];
}