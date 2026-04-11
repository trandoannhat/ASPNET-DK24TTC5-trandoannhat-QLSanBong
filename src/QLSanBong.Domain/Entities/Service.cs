using QLSanBong.Domain.Entities.Base;
using QLSanBong.Domain.Enums;

namespace QLSanBong.Domain.Entities;

//  Bảng danh mục sản phẩm/dịch vụ
public class Service : BaseEntity // BaseEntity thường chứa Id, CreatedAt...
{
    public string Name { get; set; } = string.Empty; // Nước khoáng, Bò húc, Thuê bóng...
    public decimal Price { get; set; }
    public string Unit { get; set; } = string.Empty; // Chai, Lon, Quả, Bộ...

    // Phân loại để dễ thống kê sau này (Dùng Enum)
    public ServiceCategory Category { get; set; } // Nước uống, Đồ ăn, Thuê thiết bị
}
