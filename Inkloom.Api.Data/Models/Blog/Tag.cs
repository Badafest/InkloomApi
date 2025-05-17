using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace Inkloom.Api.Data.Models;

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
                throw new ArgumentException("Tag name should be in the format of 'tag-name'");
            }
            ValidName = value;
        }
    }

    [GeneratedRegex("^[a-z]+(-[a-z]+)*$")]
    private static partial Regex TagNameRegex();

    public List<BlogTag> BlogTags { get; set; } = [];
    public List<Blog> Blogs { get; set; } = [];
}