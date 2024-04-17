namespace Inkloom.Api.Models;

public abstract class ModelBase
{
    public int Id { get; set; } = 0;
    public DateTime CreatedDate { get; private set; }
    public DateTime UpdatedDate { get; private set; }
    public DateTime DeletedDate { get; private set; }
}