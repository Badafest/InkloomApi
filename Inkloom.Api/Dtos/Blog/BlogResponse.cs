namespace Inkloom.Api.Dtos;

public class AuthorResponse
{
    public string Username { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string? Avatar { get; set; } = "";
    public string? About { get; set; } = "";
}

public class BlogPreviewResponse
{
    public int Id { get; set; }
    public AuthorResponse? Author { get; set; }

    public string Title { get; set; } = "Untitled Blog";

    public string? Subtitle { get; set; }

    public string? HeaderImage { get; set; }
    public IEnumerable<string> Tags { get; set; } = [];
    public DateTime PubllishedDate { get; set; } = DateTime.MinValue;
}

public class BlogResponse : BlogPreviewResponse
{
    public bool Public { get; set; } = true;
    public BlogStatus Status { get; set; } = BlogStatus.DRAFT;
    public IEnumerable<ContentBlock> Content { get; set; } = [];
}