using System.ComponentModel.DataAnnotations;

namespace QLSanBong.Domain.Enums;

public enum BookingStatus
{
    [Display(Name = "Chờ duyệt")]
    Pending = 0,    // Khách mới đặt, chờ admin xác nhận

    [Display(Name = "Đã xác nhận")]
    Approved = 1,   // Đã chốt sân cho khách

    [Display(Name = "Đã hủy")]
    Cancelled = 2,  // Khách bùng hoặc sân có sự cố

    [Display(Name = "Thành công")]
    Completed = 3   // Khách đã đá xong và thanh toán
}