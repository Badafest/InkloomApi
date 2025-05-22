using System.Text.Json;

namespace Inkloom.Api.Data.Models;

public enum BlogStatus { DRAFT, PUBLISHED, ARCHIVED };
public class Blog : ModelBase
{
    public DateTime PubllishedDate { get; set; }
    public bool Public { get; set; } = true;
    public BlogStatus Status { get; set; } = BlogStatus.DRAFT;
    public int AuthorId { get; set; } = 0;
    public User? Author { get; set; }
    public string Title { get; set; } = "Untitled Blog";
    public string? Subtitle { get; set; }
    public string? HeaderImage { get; set; }
    public List<BlogTag> BlogTags { get; set; } = [];
    public List<Tag> Tags { get; set; } = [];
    public string? Content { get; set; }
}

public enum ContentBlockType
{
    richText, heading, subHeading, code, blockquote, image, separator
}

public class ContentBlock
{
    public ContentBlockType Type { get; set; }
    public string Content { get; set; } = "";
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public static class BlogHelper
{
    public static IEnumerable<ContentBlock> ParseBlogContent(string content)
    {
        return JsonSerializer.Deserialize<IEnumerable<ContentBlock>>(content) ?? [];
    }
}