using QLSanBong.Domain.Enums;

namespace QLSanBong.Application.DTOs.Pitch;

public class PitchDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PitchType { get; set; } = string.Empty;
    public decimal PricePerHour { get; set; }
    public string? ImageUrl { get; set; }
}

public class PitchBookingDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;

    public Guid PitchId { get; set; }
    public string PitchName { get; set; } = string.Empty;
    public string PitchType { get; set; } = string.Empty;

    public DateTime BookingDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class CreatePitchBookingDto
{
    public Guid PitchId { get; set; }
    public DateTime BookingDate { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class UpdatePitchBookingStatusDto
{
    public Guid BookingId { get; set; }
    public BookingStatus Status { get; set; }
}

public class CreateAdminPitchBookingDto
{
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public Guid PitchId { get; set; }
    public DateTime BookingDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string? Notes { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Approved;
}

public class BookingServiceItemDto
{
    public string ServiceName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalAmount { get; set; }
}

public class PitchBookingDetailDto : PitchBookingDto
{
    public List<BookingServiceItemDto> PurchasedServices { get; set; } = new();
}