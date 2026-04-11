using QLSanBong.Domain.Enums;

namespace QLSanBong.Application.DTOs.Service;

// Dùng cho form Thêm/Sửa
public class CreateUpdateServiceDto
{
    public Guid Id { get; set; } // Nếu Guid.Empty thì là Thêm mới
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Unit { get; set; } = string.Empty;
    public ServiceCategory Category { get; set; }
}
