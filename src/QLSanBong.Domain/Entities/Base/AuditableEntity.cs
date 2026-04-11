using QLSanBong.Domain.Interfaces.Base;

namespace QLSanBong.Domain.Entities.Base; 

public abstract class AuditableEntity : BaseEntity, IAuditable
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}