using System.ComponentModel.DataAnnotations;
using QLSanBong.Domain.Enums;

namespace QLSanBong.MVC.Models;

public class CreateUpdateServiceViewModel
{
    public Guid Id { get; set; } // Nếu là Thêm mới thì ID sẽ tự động là Guid.Empty

    [Required(ErrorMessage = "Vui lòng nhập tên mặt hàng")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập đơn giá")]
    [Range(1000, 10000000, ErrorMessage = "Đơn giá phải từ 1.000đ đến 10.000.000đ")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập đơn vị tính (VD: Chai, Lon, Quả...)")]
    public string Unit { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng chọn phân loại")]
    public ServiceCategory Category { get; set; }
}