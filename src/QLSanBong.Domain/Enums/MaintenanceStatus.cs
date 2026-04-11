namespace QLSanBong.Domain.Enums;

public enum MaintenanceStatus
{
    Scheduled = 1,   // Đã lên lịch (Chưa khóa sân)
    InMaintenance = 2, // Đang sửa chữa (Sân đang bị khóa)
    Finished = 3,    // Đã xong (Sân hoạt động bình thường)
    Canceled = 4     // Hủy lịch bảo trì
}