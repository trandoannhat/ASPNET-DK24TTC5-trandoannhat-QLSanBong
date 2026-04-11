using System.ComponentModel.DataAnnotations;

namespace QLSanBong.MVC.Models;

public class EditPitchViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên sân bóng")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng chọn loại sân")]
    public string PitchType { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập giá thuê mỗi giờ")]
    [Range(10000, 5000000, ErrorMessage = "Giá thuê phải từ 10.000đ đến 5.000.000đ")]
    public decimal PricePerHour { get; set; }

    // Thêm trường Link ảnh nếu bạn muốn đổi ảnh sân (nhớ thêm ImageUrl vào UpdatePitchDto ở tầng Application nhé)
    public string? ImageUrl { get; set; }
}