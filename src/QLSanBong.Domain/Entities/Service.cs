using QLSanBong.Domain.Entities.Base;
using QLSanBong.Domain.Enums;

namespace QLSanBong.Domain.Entities;

// Danh mục các loại dịch vụ/sản phẩm tại sân
public class Service : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Unit { get; set; } = string.Empty;

    public ServiceCategory Category { get; set; }
}