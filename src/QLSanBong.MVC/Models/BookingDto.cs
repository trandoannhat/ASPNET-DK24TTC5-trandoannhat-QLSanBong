namespace QLSanBong.Application.DTOs.Booking;

public class BookingDto
{
    public Guid Id { get; set; }
    public Guid PitchId { get; set; }
    public string? PitchName { get; set; }
    public DateTime StartTime { get; set; }
    public int DurationHours { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string? Status { get; set; }
}