namespace QLSanBong.Common.Exceptions;

public static class AppConstants
{
    public static class Roles
    {
        // 1. Định nghĩa các quyền đơn lẻ (Phải khớp với tên trong Enum UserRole)
        public const string Admin = "Admin";
        public const string Manager = "Manager";
        public const string Staff = "Staff";
        public const string Customer = "Customer";

        // 2. Định nghĩa các nhóm quyền để dùng trong [Authorize]
        // Nhóm này dành cho những người có quyền quản lý sân (Chủ sân và Quản lý)
        public const string PitchManagers = $"{Admin},{Manager}";

        // Nhóm này dành cho tất cả nhân viên nội bộ (bao gồm cả nhân viên trực sân)
        public const string InternalAccess = $"{Admin},{Manager},{Staff}";
    }
}