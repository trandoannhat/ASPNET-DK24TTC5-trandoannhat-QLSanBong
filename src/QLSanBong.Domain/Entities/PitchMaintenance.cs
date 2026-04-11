using QLSanBong.Domain.Entities.Base;
using QLSanBong.Domain.Enums;

namespace QLSanBong.Domain.Entities;

// Theo dõi lịch bảo trì và sửa chữa sân bóng
public class PitchMaintenance : BaseEntity
{
    public Guid PitchId { get; set; }
    public Pitch Pitch { get; set; }

    public string Reason { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public decimal EstimatedCost { get; set; }

    public MaintenanceStatus Status { get; set; }
}