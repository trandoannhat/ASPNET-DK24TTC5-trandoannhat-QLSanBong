using QLSanBong.Domain.Interfaces.Base;
using QLSanBong.Domain.Entities.Base;

namespace QLSanBong.Domain.Entities;

public class Pitch : BaseEntity, IAuditable, ISoftDelete
{
    public string Name { get; set; } = string.Empty;
    public string PitchType { get; set; } = string.Empty; // VD: Sân 5, Sân 7
    public decimal PricePerHour { get; set; }
    public string? ImageUrl { get; set; }
    // IAuditable
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // ISoftDelete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public void Undo()
    {
        IsDeleted = false;
        DeletedAt = null;
    }

    public ICollection<PitchBooking> PitchBookings { get; set; } = new List<PitchBooking>();
}