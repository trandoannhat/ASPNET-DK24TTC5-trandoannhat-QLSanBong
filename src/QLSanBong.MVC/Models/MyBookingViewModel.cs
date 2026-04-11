namespace QLSanBong.MVC.Models;

public class MyBookingViewModel
{
    public Guid Id { get; set; } // ID của luồng đặt sân (để sau này làm chức năng Hủy)
    public string PitchName { get; set; }
    public DateTime BookingDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } // Ví dụ: "Chờ Xác Nhận", "Đã Duyệt", "Đã Hủy"
    public string? Notes { get; set; }
}
