
using System.Text.RegularExpressions;

namespace Inkloom.Api.Services;

public partial class TagService(DataContext context) : ITagService
{
    private readonly DataContext _context = context;
    public async Task<ServiceResponse<IEnumerable<string>>> GetTags(string searchText)
    {
        if (InvalidSearchRegex().IsMatch(searchText))
        {
            throw new ArgumentException("Tag names contain letters and dashes only");
        }
        if (searchText.Length < 2)
        {
            throw new ArgumentException("Too short search string");
        }
        var tags = await _context.Tags
            .Where(tag => tag.Name.Contains(searchText.ToLower()))
            .Include(tag => tag.BlogTags)
            .OrderByDescending(tag => tag.BlogTags.Count)
            .Take(100)
            .Select(tag => tag.Name)
            .ToArrayAsync();

        return new() { Data = tags };
    }

    [GeneratedRegex("[^a-zA-Z-]")]
    private static partial Regex InvalidSearchRegex();
}
