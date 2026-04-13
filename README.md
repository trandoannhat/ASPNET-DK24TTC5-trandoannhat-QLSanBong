# XÂY DỰNG WEBSITE QUẢN LÝ SÂN BÓNG ĐÁ MINI (QLSanBong)

## 1. Giới thiệu đề tài

Dự án QLSanBong được xây dựng nhằm số hóa toàn diện quy trình vận hành sân bóng đá mini, giải quyết các bài toán thực tế như:

- Tự động hóa việc quản lý lịch đặt sân, chống trùng lịch (Overbooking).
- Kiểm soát doanh thu chi tiết từ tiền sân đến các dịch vụ đi kèm (Canteen).
- Tích hợp thanh toán trực tuyến hiện đại, mang lại trải nghiệm chuyên nghiệp cho người chơi.

---

## 2. Công nghệ và Kiến trúc

- Backend: ASP.NET Core 8.0 (Web API & MVC).
- Database: SQL Server & Entity Framework Core (Code-First).
- Kiến trúc: Clean Architecture (5 lớp), Repository Pattern & Unit of Work.
- Xác thực: JWT Bearer Token (API) & Cookie Authentication (MVC).
- Thanh toán: Tích hợp cổng thanh toán VNPay.
- Giao diện: Bootstrap 5, AJAX, SweetAlert2 & FullCalendar.
- Mapping: AutoMapper.

---

## 3. Cấu trúc Project (Clean Architecture)

Dự án được phân tách rõ ràng thành các tầng chuyên biệt:

- QLSanBong.Domain: Chứa các thực thể cốt lõi (User, Pitch, PitchBooking, Service,...).
- QLSanBong.Application: Chứa Interfaces, Services xử lý nghiệp vụ, DTOs và Mapping Profiles.
- QLSanBong.Infrastructure: Xử lý Database (DbContext, Repository, Unit of Work), tích hợp VNPay và các Service hạ tầng.
- QLSanBong.API: Cung cấp hệ thống RESTful API cho các ứng dụng mở rộng.
- QLSanBong.MVC: Ứng dụng Web hoàn chỉnh dành cho Quản trị viên và Khách hàng.

---

## 4. Các tính năng chính

### Phân hệ Quản trị (Admin / PitchAdmin)

- Dashboard: Biểu đồ thống kê doanh thu theo ngày/tháng, theo dõi hiệu suất kinh doanh.
- Lịch biểu (Calendar): Quản lý trạng thái sân trực quan trên giao diện FullCalendar.
- Quản lý danh mục: Quản lý sân bóng, loại sân, và các dịch vụ Canteen.
- Xử lý đơn hàng: Duyệt/Hủy lịch đặt, ghi chú hóa đơn và bán thêm dịch vụ trực tiếp.

### Phân hệ Khách hàng (Customer)

- Tra cứu: Tìm kiếm sân trống theo ngày, khung giờ và loại sân.
- Đặt sân và Thanh toán: Đặt sân trực tuyến, nhận thông báo và thanh toán đặt cọc qua VNPay.
- Cá nhân: Quản lý hồ sơ, lịch sử đặt sân và trạng thái hóa đơn.

---

## 5. Hướng dẫn cài đặt và Chạy dự án

### Bước 1: Clone mã nguồn

git clone https://github.com/trandoannhat/ASPNET-DK24TTC5-trandoannhat-QLSanBong.git

### Bước 2: Cấu hình Database

Mở file appsettings.json trong project QLSanBong.API hoặc QLSanBong.MVC, điều chỉnh chuỗi kết nối phù hợp với SQL Server của bạn:
"ConnectionStrings": {
"DefaultConnection": "Server=.;Database=QLSanBong;Trusted_Connection=True;TrustServerCertificate=True;"
}

### Bước 3: Cập nhật Database (Migration)

Bạn có thể lựa chọn 1 trong 2 cách sau để thiết lập CSDL:

Cách 1: Chạy tự động (Khuyên dùng)
Dự án tích hợp sẵn Auto-Migration và Data Seeder. Khi khởi chạy, hệ thống sẽ tự động tạo cấu trúc bảng và nạp dữ liệu mẫu.

1. Mở Solution bằng Visual Studio 2022.
2. Thiết lập Multiple startup projects cho cả QLSanBong.API và QLSanBong.MVC.
3. Nhấn F5. Hệ thống sẽ tự động khởi tạo Database sau vài giây.

Cách 2: Sử dụng Script SQL (Thủ công)

1. Mở SQL Server Management Studio (SSMS).
2. Chạy file script src/Database/QLSanBong_Setup.sql để tạo Database và Data mẫu.

Lưu ý: Script được xuất từ SQL Server 2019/2022. Nếu sử dụng phiên bản cũ hơn (2012, 2014) bị báo lỗi cú pháp, vui lòng sử dụng Cách 1 để đảm bảo tính tương thích.

### Bước 4: Chạy ứng dụng

1. Mở Solution bằng Visual Studio 2022.
2. Click chuột phải vào Solution -> Chọn Set Startup Projects...
3. Chọn Multiple startup projects cho 2 project sau:
   . QLSanBong.API (Action: Start)
   . QLSanBong.MVC (Action: Start)
4. Nhấn F5 để khởi chạy hệ thống.

## 6. Tài khoản dùng thử (Demo Data)

Hệ thống đã được nạp sẵn dữ liệu mẫu để thuận tiện cho việc kiểm tra:

1. Quản trị viên (Admin):

- Email: admin@qlsanbong.com
- Mật khẩu: admin123@A
- Quyền hạn: Toàn quyền hệ thống, Dashboard, Quản lý lịch biểu và Dịch vụ.

2. Khách hàng (Customer):

- Email: khachhang@gmail.com
- Mật khẩu: client123@A
- Quyền hạn: Tìm kiếm sân, Đặt lịch, Thanh toán và xem Lịch sử cá nhân.

---

## 7. Thông tin sinh viên thực hiện

- Họ và tên: Trần Doãn Nhất
- Lớp: DK24TTC5
- Môn học: Chuyên đề Web ASP.NET
- Đề tài: Xây dựng website quản lý sân bóng đá mini (QLSanBong)
- Email: doannhatit@gmail.com
- Số điện thoại: 0907.011.886
