using System.ComponentModel.DataAnnotations;

namespace QLSanBong.MVC.Models;

public class BookPitchViewModel
{
    public Guid PitchId { get; set; }
    public string? PitchName { get; set; }
    public decimal PricePerHour { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên của bạn")]
    public string CustomerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại để chúng tôi liên hệ")]
    public string CustomerPhone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng chọn ngày đặt")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime BookingDate { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Vui lòng nhập giờ bắt đầu (VD: 14:00)")]
    [RegularExpression(@"^([01][0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Sai định dạng giờ (HH:mm)")]
    public string StartTime { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập giờ kết thúc (VD: 15:30)")]
    [RegularExpression(@"^([01][0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Sai định dạng giờ (HH:mm)")]
    public string EndTime { get; set; } = string.Empty;

    public string? Notes { get; set; }

    // THUỘC TÍNH MỚI: LƯU PHƯƠNG THỨC THANH TOÁN
    public string PaymentMethod { get; set; } = "Tiền mặt";
}