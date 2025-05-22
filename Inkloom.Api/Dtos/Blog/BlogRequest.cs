using System.Text.Json.Nodes;

namespace Inkloom.Api.Dtos;

public class BlogRequest
{
    public bool? Public { get; set; } = true;
    public BlogStatus? Status { get; set; } = BlogStatus.DRAFT;
    public string Title { get; set; } = "Untitled Blog";
    public string? Subtitle { get; set; }
    public string? HeaderImage { get; set; }
    public IEnumerable<string>? Tags { get; set; }
    public IEnumerable<string>? Content { get; set; }
}