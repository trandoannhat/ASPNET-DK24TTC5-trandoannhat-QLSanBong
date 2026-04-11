using QLSanBong.Domain.Entities.Base;

namespace QLSanBong.Domain.Entities;

//  Bảng trung gian lưu chi tiết hóa đơn (Booking <-> Service)
public class BookingService : BaseEntity
{
    public Guid BookingId { get; set; }
    public PitchBooking Booking { get; set; } // Navigation property

    public Guid ServiceId { get; set; }
    public Service Service { get; set; } // Navigation property

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; } // Lưu lại giá tại thời điểm bán (đề phòng giá gốc thay đổi)

    // Tính tiền = Quantity * UnitPrice
    public decimal TotalAmount => Quantity * UnitPrice;
}
