using System.ComponentModel.DataAnnotations;

namespace QLSanBong.MVC.Models;

public class BookingViewModel
{
    public Guid Id { get; set; } // Phục vụ cho Quản trị viên

    public string? Status { get; set; } // Trạng thái: Chờ Xác Nhận, Đã Duyệt...

    public Guid PitchId { get; set; }

    public string? PitchName { get; set; }

    // --- BỔ SUNG: Dữ liệu cần thiết để Admin hiển thị trên bảng ---
    public DateTime BookingDate { get; set; } // Ngày đá
    public string? EndTime { get; set; }      // Giờ kết thúc (VD: 16:00)
    public decimal TotalPrice { get; set; }   // Tổng tiền
    // --------------------------------------------------------------

    [Required(ErrorMessage = "Vui lòng chọn thời gian bắt đầu")]
    // ĐỔI SANG STRING ĐỂ FIX LỖI AUTOMAPPER VÀ KHỚP VỚI <input type="time">
    public string StartTime { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập số giờ đá")]
    [Range(1, 5, ErrorMessage = "Số giờ đá từ 1 đến 5 giờ")]
    public int DurationHours { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên người đặt")]
    public string CustomerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    public string CustomerPhone { get; set; } = string.Empty;

    public string? Notes { get; set; }
}