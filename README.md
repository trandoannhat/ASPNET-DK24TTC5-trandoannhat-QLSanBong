# 🏟️ HỆ THỐNG QUẢN LÝ SÂN BÓNG MINI (QLSanBong)

## 1. Giới thiệu đề tài

Trong bối cảnh nhu cầu thuê sân bóng đá mini ngày càng tăng cao, việc quản lý thủ công thường dẫn đến các sai sót như:

- Trùng lịch đặt sân
- Khó kiểm soát doanh thu dịch vụ
- Thiếu thông tin khách hàng

Dự án **QLSanBong** được xây dựng nhằm số hóa toàn diện quy trình vận hành, giúp chủ sân tối ưu hóa việc kinh doanh và mang lại trải nghiệm chuyên nghiệp cho người chơi.

---

## 2. Công nghệ sử dụng

- Backend: ASP.NET Core 8.0 (Web API & MVC)
- Database: SQL Server & Entity Framework Core (Code-First)
- Kiến trúc: Clean Architecture, Repository Pattern & Unit of Work
- Xác thực: JWT Bearer Token & Cookie Authentication
- Thanh toán: VNPay
- Giao diện: Bootstrap 5, AJAX & FullCalendar
- Mapping: AutoMapper

---

## 3. Kiến trúc hệ thống (Clean Architecture)

Dự án được chia thành 5 lớp:

- **QLSanBong.Domain**  
  Chứa các thực thể: `User`, `Pitch`, `PitchBooking`, `Service`

- **QLSanBong.Application**  
  Chứa Services, DTOs, Mapping

- **QLSanBong.Infrastructure**  
  DbContext, Repository, tích hợp VNPay, Upload file

- **QLSanBong.API**  
  RESTful API

- **QLSanBong.MVC**  
  Giao diện Web

---

## 4. Các tính năng nổi bật

### 👨‍💼 Đối với Quản trị viên (Admin / Manager)

- Dashboard: thống kê doanh thu, biểu đồ 7 ngày
- Calendar: xem lịch sân trực quan
- Quản lý sân: thêm / sửa / xóa / upload ảnh
- Canteen: thêm dịch vụ vào hóa đơn
- Xử lý đặt sân: duyệt / hủy / tạo nhanh

### 🏃‍♂️ Đối với Khách hàng (Customer)

- Tra cứu sân theo ngày và giờ
- Đặt sân online + cọc 30% (VNPay)
- Xem lịch sử và hóa đơn

---

## 5. Hướng dẫn cài đặt & chạy project

### Bước 1: Clone source code

\`\`\`bash
git clone https://github.com/trandoannhat/ASPNET-DK24TTC5-trandoannhat-QLSanBong.git
cd ASPNET-DK24TTC5-trandoannhat-QLSanBong
\`\`\`

### Bước 2: Cấu hình Database

Mở file \`appsettings.json\` trong project **QLSanBong.API** (hoặc MVC):

\`\`\`json
"ConnectionStrings": {
"DefaultConnection": "Server=.;Database=QLSanBong;Trusted_Connection=True;TrustServerCertificate=True;"
}
\`\`\`

### Bước 3: Cập nhật Database (Migration)

\`\`\`bash
dotnet ef database update
\`\`\`

> Hệ thống có DataSeeder tự tạo dữ liệu mẫu

### Bước 4: Chạy ứng dụng

- Chọn Multiple Startup Projects:
  - QLSanBong.API
  - QLSanBong.MVC

👉 Nhấn **F5** để chạy

---

## 6. Thông tin sinh viên thực hiện

- Họ tên: Trần Doãn Nhất
- Môn học: Đồ án Web ASP.NET
- Đề tài: Website quản lý sân bóng mini
- Email: doannhatit@gmail.com
- SĐT: 0907.011.886
