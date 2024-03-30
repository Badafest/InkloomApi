using System.ComponentModel.DataAnnotations.Schema;

namespace InkloomApi.Models;

public abstract class ModelBase
{
    public int Id { get; set; } = 0;

    public DateTimeOffset CreatedDate { get; private set; }

    public int CreatedById { get; set; } = 0;

    [ForeignKey("CreatedById")]
    public User? CreatedBy { get; set; }

    public DateTimeOffset UpdatedDate { get; private set; }
    public int UpdatedById { get; set; } = 0;
    [ForeignKey("UpdatedById")]
    public User? UpdatedBy { get; set; }
    public DateTimeOffset DeletedDate { get; private set; }
    public int DeletedById { get; set; } = 0;
    [ForeignKey("DeletedById")]
    public User? DeletedBy { get; set; }
}