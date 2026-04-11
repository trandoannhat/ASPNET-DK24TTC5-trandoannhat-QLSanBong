using QLSanBong.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace QLSanBong.MVC.Models
{
    public class AdminBookPitchViewModel
    {
        public Guid PitchId { get; set; }
        public string PitchName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập tên khách hàng")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập SĐT khách hàng")]
        public string CustomerPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn ngày đặt")]
        [DataType(DataType.Date)] // Khai báo rõ đây là kiểu Ngày (không lấy Giờ)
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)] // Ép chuẩn định dạng HTML5
        public DateTime BookingDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Vui lòng nhập giờ bắt đầu")]
        [RegularExpression(@"^([01][0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Sai định dạng giờ (HH:mm)")]
        public string StartTime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập giờ kết thúc")]
        [RegularExpression(@"^([01][0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Sai định dạng giờ (HH:mm)")]
        public string EndTime { get; set; } = string.Empty;

        public string? Notes { get; set; }

        public BookingStatus Status { get; set; } = BookingStatus.Approved; // Admin tạo thì mặc định là Đã xác nhận
    }
}
