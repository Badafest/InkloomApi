
using System.Text.RegularExpressions;

namespace InkloomApi.Services;

public partial class TagService(DataContext context) : ITagService
{
    private readonly DataContext _context = context;
    public async Task<ServiceResponse<IEnumerable<string>>> GetTags(string searchText)
    {
        if (InvalidSearchRegex().IsMatch(searchText))
        {
            throw new ArgumentException("Tag names contain letters only");
        }
        var tags = await _context.Tags
            .Include(tag => Regex.IsMatch(tag.Name, searchText))
            .OrderByDescending(tag => tag.Id)
            .Take(100)
            .Select(tag => tag.Name)
            .ToArrayAsync();

        return new() { Data = tags };
    }

    [GeneratedRegex("^[a-zA-Z]")]
    private static partial Regex InvalidSearchRegex();
}
