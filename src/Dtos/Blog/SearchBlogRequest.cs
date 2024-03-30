namespace InkloomApi.Dtos;

public class SearchPublicBlogRequest
{
    public string[]? Tags { get; set; }

    public bool? Status { get; set; }

    public string? Author { get; set; }
    public string? SearchText { get; set; }

    public int Page { get; set; } = 1;
}