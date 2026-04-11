namespace QLSanBong.Domain.Enums;

public enum MaintenanceStatus
{
    Pending = 1,        // Đã lên lịch / Chờ xử lý (Sân sẽ bị khóa trong tương lai)
    InProgress = 2,     // Đang tiến hành sửa chữa (Sân đang khóa)
    Completed = 3,      // Đã hoàn thành (Mở khóa sân trở lại)
    Cancelled = 4       // Đã hủy lịch bảo trì (Không sửa nữa, mở khóa sân)
}