using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace Inkloom.Api.Models;

public partial class Tag
{
    public int Id { get; set; }
    [NotMapped]
    public string ValidName = "";
    public string Name
    {
        get
        {
            return ValidName;
        }
        set
        {
            if (!TagNameRegex().IsMatch(value))
            {
                throw new ArgumentException("Tag name should be at least 3 characters long and should contain uppercase letters only");
            }
            ValidName = value;
        }
    }

    [GeneratedRegex("^[A-Z]{3,}$")]
    private static partial Regex TagNameRegex();

    public List<BlogTag> BlogTags { get; set; } = [];
    public List<Blog> Blogs { get; set; } = [];
}