using QLSanBong.Domain.Interfaces.Base;
using QLSanBong.Domain.Entities.Base;

namespace QLSanBong.Domain.Entities;

// Thông tin chi tiết sân bóng (Sân 5, sân 7...)
public class Pitch : BaseEntity, IAuditable, ISoftDelete
{
    public string Name { get; set; } = string.Empty;
    public string PitchType { get; set; } = string.Empty;
    public decimal PricePerHour { get; set; }
    public string? ImageUrl { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public void Undo()
    {
        IsDeleted = false;
        DeletedAt = null;
    }

    public ICollection<PitchBooking> PitchBookings { get; set; } = new List<PitchBooking>();
}