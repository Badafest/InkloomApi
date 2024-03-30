namespace InkloomApi.Dtos;

public abstract class SearchBlogRequestBase
{
    public string[]? Tags { get; set; }

    public string? SearchText { get; set; }

    public int Page { get; set; } = 1;
}

public class SearchOwnBlogRequest : SearchBlogRequestBase
{
    public BlogStatus? Status { get; set; }

    public bool? Public { get; set; }
}

public class SearchPublicBlogRequest : SearchBlogRequestBase
{
    public string? Author { get; set; }
}

public class SearchBlogRequest : SearchBlogRequestBase
{
    public SearchBlogRequest(SearchOwnBlogRequest ownSearchRequest)
    {
        Status = ownSearchRequest.Status;
        Public = ownSearchRequest.Public;
    }

    public SearchBlogRequest(SearchPublicBlogRequest publicBlogRequest)
    {
        Author = publicBlogRequest.Author;
        Public = true;
        Status = BlogStatus.PUBLISHED;
    }
    public BlogStatus? Status { get; set; }
    public string? Author { get; set; }

    public bool? Public { get; set; }
}