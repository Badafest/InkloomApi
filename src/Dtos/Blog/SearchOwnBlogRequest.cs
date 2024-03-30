namespace InkloomApi.Dtos;

public class SearchOwnBlogRequest
{
    public string[]? Tags { get; set; }

    public bool? Public { get; set; }

    public bool? Status { get; set; }

    public string? SearchText { get; set; }

    public int Page { get; set; } = 1;
}