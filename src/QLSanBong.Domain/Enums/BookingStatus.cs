using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace QLSanBong.Domain.Enums;

public enum BookingStatus
{


    [Display(Name = "Chờ Xác Nhận")]
    Pending = 0,// Chờ xác nhận

    [Display(Name = "Đã Duyệt (Thành công)")]
    Approved = 1,// Đã xác nhận (đã xếp lịch)

    [Display(Name = "Đã Hủy")]
    Cancelled = 2,// Đã hủy

    [Display(Name = "Đã Hoàn Thành")]
    Completed = 3  // Đã hoàn thành (đã phục vụ xong)
}
