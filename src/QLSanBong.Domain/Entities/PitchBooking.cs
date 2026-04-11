using QLSanBong.Domain.Enums;
using QLSanBong.Domain.Interfaces.Base;
using QLSanBong.Domain.Entities.Base;

namespace QLSanBong.Domain.Entities;

// Thông tin khách đặt lịch sân
public class PitchBooking : BaseEntity, IAuditable, ISoftDelete
{
    public Guid UserId { get; set; }
    public User User { get; set; }

    public Guid PitchId { get; set; }
    public Pitch Pitch { get; set; }

    public DateTime BookingDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; set; }
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ICollection<BookingService> BookingServices { get; set; } = new List<BookingService>();

    public void Undo()
    {
        IsDeleted = false;
        DeletedAt = null;
    }
}