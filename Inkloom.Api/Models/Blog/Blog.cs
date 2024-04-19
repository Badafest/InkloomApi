namespace Inkloom.Api.Models;

public enum BlogStatus { DRAFT, PUBLISHED, ARCHIVED };
public class Blog : ModelBase
{
    public DateTime PubllishedDate { get; set; }
    public bool Public { get; set; } = true;
    public BlogStatus Status { get; set; } = BlogStatus.DRAFT;
    public int AuthorId { get; set; } = 0;
    public User? Author { get; set; }

    public string Title { get; set; } = "Untitled Blog";

    public string? Description { get; set; }

    public string? HeaderImage { get; set; }

    public List<BlogTag> BlogTags { get; set; } = [];
    public List<Tag> Tags { get; set; } = [];
    public string? Content { get; set; }

}