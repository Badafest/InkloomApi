namespace InkloomApi.Dtos;

public class UpdateBlogRequest
{
    public bool? Public { get; set; } = true;
    public BlogStatus? Status { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? HeaderImage { get; set; }
    public List<string>? Tags { get; set; } = [];

    public string? Content { get; set; }
}