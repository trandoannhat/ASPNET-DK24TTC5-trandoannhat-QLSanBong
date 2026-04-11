using QLSanBong.Domain.Entities.Base;
using QLSanBong.Domain.Enums;

namespace QLSanBong.Domain.Entities;

public class PitchMaintenance : BaseEntity
{
    public Guid PitchId { get; set; }
    public Pitch Pitch { get; set; }

    public string Reason { get; set; } = string.Empty; // VD: Hỏng dàn đèn pha sân số 1

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public decimal EstimatedCost { get; set; } // Chi phí dự kiến để tính toán lợi nhuận/lỗ

    public MaintenanceStatus Status { get; set; } // Enum: Đang chờ, Đang sửa, Đã hoàn thành
}
