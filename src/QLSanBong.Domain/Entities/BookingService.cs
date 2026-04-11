using QLSanBong.Domain.Entities.Base;

namespace QLSanBong.Domain.Entities;

// Lưu thông tin chi tiết các dịch vụ sử dụng trong 1 lần đặt sân
public class BookingService : BaseEntity
{
    public Guid BookingId { get; set; }
    public PitchBooking Booking { get; set; }

    public Guid ServiceId { get; set; }
    public Service Service { get; set; }

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; } // Giá tại thời điểm bán

    public decimal TotalAmount => Quantity * UnitPrice;
}