namespace Domain.Entities;

public class ObjectEntity : BaseEntity
{
    public DateTime CreateDate { get; set; } = DateTime.Now;
    public DateTime LeastUpdate { get; set; }
}