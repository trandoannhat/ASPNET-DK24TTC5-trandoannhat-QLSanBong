🏟️ HỆ THỐNG QUẢN LÝ SÂN BÓNG MINI (QLSanBong)

1. Giới thiệu đề tài
   Trong bối cảnh nhu cầu thuê sân bóng đá mini ngày càng tăng cao, việc quản lý thủ công thường dẫn đến các sai sót như trùng lịch, khó kiểm soát doanh thu dịch vụ và thiếu thông tin khách hàng. Dự án QLSanBong được xây dựng nhằm số hóa toàn diện quy trình vận hành, giúp chủ sân tối ưu hóa việc kinh doanh và mang lại trải nghiệm chuyên nghiệp cho người chơi.

2. Công nghệ sử dụng
   Backend: ASP.NET Core 8.0 (Web API & MVC).

   Database: SQL Server & Entity Framework Core (Code-First).

   Kiến trúc: Clean Architecture, Repository Pattern & Unit of Work.

   Xác thực: JWT Bearer Token & Cookie Authentication.

   Thanh toán: Tích hợp cổng thanh toán trực tuyến VNPay.

   Giao diện: Bootstrap 5, AJAX & thư viện FullCalendar (vẽ lịch thi đấu trực quan).

Mapping: AutoMapper.

3. Kiến trúc hệ thống (Clean Architecture)
   Dự án được phân chia thành 5 lớp rõ rệt nhằm đảm bảo tính bảo trì và mở rộng:

QLSanBong.Domain: Chứa các thực thể (Entities) cốt lõi như User, Pitch, PitchBooking, Service và các Interface nền tảng.

QLSanBong.Application: Chứa logic nghiệp vụ (Services), DTOs và cấu hình Mapping.

QLSanBong.Infrastructure: Xử lý kết nối Database (DbContext), triển khai Repositories và các dịch vụ ngoại vi như File Upload, VNPay.

QLSanBong.API: Cung cấp hệ thống RESTful API cho đa nền tảng.

QLSanBong.MVC: Giao diện Web dành cho người dùng cuối và quản trị viên.

4. Các tính năng nổi bật đã triển khai
   👨‍💼 Đối với Quản trị viên (Admin/Manager)
   Dashboard: Thống kê doanh thu thực tế, số lượng đặt sân theo ngày/tháng và biểu đồ tăng trưởng 7 ngày gần nhất.

   Lịch biểu (Calendar): Theo dõi toàn bộ ca đá trong tuần/tháng dưới dạng lưới thời gian trực quan bằng FullCalendar.

   Quản lý Sân bóng: Thêm, sửa, xóa thông tin sân (tên, loại sân 5/7/11, giá tiền) và upload ảnh thực tế.

   Canteen & Dịch vụ: Thêm nước uống, thuê bóng, thuê áo pit vào hóa đơn trực tiếp cho từng ca đá của khách.

   Xử lý Đặt sân: Duyệt hoặc hủy lịch đặt online, đồng thời hỗ trợ tạo lịch nhanh cho khách vãng lai (Offline).

🏃‍♂️ Đối với Khách hàng (Customer)
Tra cứu sân: Tìm kiếm sân trống theo ngày và khung giờ mong muốn.

      Đặt sân & Thanh toán: Đặt sân trực tuyến và thực hiện thanh toán cọc (30%) qua cổng VNPay để giữ chỗ tự động.

      Quản lý cá nhân: Theo dõi lịch sử đặt sân và trạng thái hóa đơn.

5. Hướng dẫn cài đặt & Chạy project
   Bước 1: Clone source code
   Bash
   git clone https://github.com/trandoannhat/ASPNET-DK24TTC5-trandoannhat-QLSanBong.git
   cd ASPNET-DK24TTC5-trandoannhat-QLSanBong
   Bước 2: Cấu hình Database
   Mở file appsettings.json tại project QLSanBong.API (hoặc MVC) và chỉnh lại chuỗi kết nối:

JSON
"ConnectionStrings": {
"DefaultConnection": "Server=.;Database=QLSanBong;Trusted_Connection=True;TrustServerCertificate=True;"
}
Bước 3: Cập nhật Database (Migration)
Chạy lệnh sau trong Package Manager Console:

Bash
dotnet ef database update
(Hệ thống đã tích hợp DataSeeder, tự động tạo tài khoản Admin và dữ liệu sân mẫu ở lần chạy đầu tiên).

Bước 4: Chạy ứng dụng
Chọn Multiple Startup Projects cho cả QLSanBong.API và QLSanBong.MVC.

Nhấn F5 để khởi động.

6. Thông tin sinh viên thực hiện
   Họ tên: Trần Doãn Nhất

   Môn học: Đồ án Web ASP.NET

   Đề tài: Xây dựng website quản lý sân bóng đá mini

   Email: doannhatit@gmail.com

   SĐT: 0907.011.886
