namespace Inkloom.Api.Dtos;

public class UpdateBlogRequest
{
    public bool? Public { get; set; }
    public BlogStatus? Status { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? HeaderImage { get; set; }
    public IEnumerable<TagResponse>? Tags { get; set; }

    public string? Content { get; set; }
}