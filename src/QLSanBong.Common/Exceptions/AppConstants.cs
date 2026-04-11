namespace QLSanBong.Common.Exceptions;

public static class AppConstants
{
    // Gom nhóm các hằng số về Role
    public static class Roles
    {
        // 1. CÁC QUYỀN ĐƠN LẺ (Tên chuỗi phải khớp với Enum UserRole)
        public const string Admin = "Admin";
       
       
        public const string PitchAdmin = "PitchAdmin";

                // Dùng cho module quản lý Sân bóng 
        public const string PitchManagers = $"{Admin},{PitchAdmin}";
    }

    
}