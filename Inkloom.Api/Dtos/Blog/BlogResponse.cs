namespace Inkloom.Api.Dtos;

public class AuthorResponse
{
    public string Username { get; set; } = "";
    public string Avatar { get; set; } = "";
    public string About { get; set; } = "";
}

public class BlogPreviewResponse
{
    public AuthorResponse? Author { get; set; }

    public string Title { get; set; } = "Untitled Blog";

    public string? Description { get; set; }

    public string? HeaderImage { get; set; }
    public IEnumerable<string> Tags { get; set; } = [];
}

public class BlogResponse : BlogPreviewResponse
{
    public bool Public { get; set; } = true;
    public BlogStatus Status { get; set; } = BlogStatus.DRAFT;

    public string? Content { get; set; }
}