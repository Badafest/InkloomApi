using System.Text.Json;
using SoftCircuits.HtmlMonkey;

namespace Inkloom.Api.Data.Models;

public enum BlogStatus { DRAFT, PUBLISHED, ARCHIVED };
public class Blog : ModelBase
{
    public DateTime PublishedDate { get; set; }
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
    public int ReadingTime
    {
        get
        {
            var parsedHtml = BlogHelper.ParseBlogContent(Content ?? "");
            var wordCount = CountWordsInHtml(string.Join(' ', parsedHtml.Select(block => block.Content)));
            var rawReadingTime = wordCount / 200; // average reading rate of 250 words per minute
            return (int)((Math.Round(rawReadingTime / 5f) + 1) * 5); // round to nearest multiple of 5 minutes
        }
    }
    private static int CountWordsInHtml(string html)
    {
        var count = 0;
        var doc = HtmlDocument.FromHtml(html);
        foreach (var node in doc.RootNodes)
        {
            var words = node.Text.Split(' ').Where(word => !string.IsNullOrEmpty(word));
            count += words.Count();
        }
        return count;
    }
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

    public static IEnumerable<ContentBlock> ParseBlogContent(IEnumerable<string> content)
    {
        return content.Select(block =>
        {
            var parsedBlock = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(block)!;
            return new ContentBlock
            {
                Type = (ContentBlockType)Enum.Parse(typeof(ContentBlockType), parsedBlock["type"].ToString()),
                Content = parsedBlock["content"].ToString(),
                Metadata = JsonSerializer.Deserialize<Dictionary<string, string>>(parsedBlock["metadata"].ToString())
            };
        });
    }
}