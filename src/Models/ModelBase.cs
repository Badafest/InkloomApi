using System.ComponentModel.DataAnnotations.Schema;

namespace InkloomApi.Models;

public abstract class ModelBase
{
    public int Id { get; set; } = 0;

    public DateTime CreatedDate { get; private set; }

    public int? CreatedById { get; set; }

    [ForeignKey("CreatedById")]
    public User? CreatedBy { get; set; }

    public DateTime UpdatedDate { get; private set; }
    public int? UpdatedById { get; set; }
    [ForeignKey("UpdatedById")]
    public User? UpdatedBy { get; set; }
    public DateTime DeletedDate { get; private set; }
    public int? DeletedById { get; set; }
    [ForeignKey("DeletedById")]
    public User? DeletedBy { get; set; }
}