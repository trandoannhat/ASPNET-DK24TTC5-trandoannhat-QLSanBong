using System.Text.Json.Serialization;

namespace QLSanBong.Domain.Enums;

[Flags] // BẮT BUỘC PHẢI THÊM DÒNG NÀY
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserRole
{
    None = 0,         // Không có quyền gì (Giá trị mặc định của Flags)

    // --- CÁC QUYỀN CŨ (Giữ nguyên tên để DB không bị lỗi) ---
    Client = 1,       // Khách hàng (Đổi từ 2 thành 1)
    Admin = 2,        // Boss tổng (Đổi từ 0 thành 2)
    Editor = 4,       // Người viết bài (Đổi từ 1 thành 4)

    // --- CÁC QUYỀN MỚI THÊM VÀO ---
    SpaAdmin = 8,     // Quản lý Spa
    PitchAdmin = 16   // Quản lý Sân bóng (Tương lai)
}

//Trong kỹ thuật[Flags] (Cờ bit), các giá trị bắt buộc phải là lũy thừa của 2 (1, 2, 4, 8, 16...).
//Giá trị 0 luôn được dành cho None(Không có quyền gì).