using QLSanBong.Domain.Enums;

namespace QLSanBong.Application.DTOs.Service;

public class ServiceDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Unit { get; set; } = string.Empty; // VD: Chai, Lon, Quả
    public ServiceCategory Category { get; set; } // Phân loại (Nước, Đồ ăn...)
}
