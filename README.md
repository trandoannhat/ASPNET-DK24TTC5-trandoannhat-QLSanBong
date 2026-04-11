# ⚽ ĐỒ ÁN MÔN HỌC ASP.NET

## HỆ THỐNG QUẢN LÝ SÂN BÓNG (QLSanBong)

---

## 1. Giới thiệu đề tài

Trong bối cảnh nhu cầu thuê sân bóng ngày càng tăng, việc quản lý thủ công gây nhiều khó khăn như:

- Trùng lịch đặt sân
- Khó kiểm soát doanh thu
- Thiếu thông tin khách hàng

Đề tài **Xây dựng website quản lý sân bóng đá mini(QLSanBong)** được xây dựng nhằm:

- Tin học hóa quy trình quản lý sân bóng
- Giảm sai sót trong vận hành
- Tăng hiệu quả quản lý và theo dõi

---

## 2. Mục tiêu hệ thống

- Xây dựng hệ thống quản lý sân bóng bằng ASP.NET
- Áp dụng mô hình **Clean Architecture**
- Thực hành xây dựng Web API + MVC
- Kết nối và thao tác với cơ sở dữ liệu SQL Server

---

## 3. Công nghệ sử dụng

- ASP.NET Core (.NET 8)
- Entity Framework Core
- SQL Server
- ASP.NET MVC
- RESTful API
- AutoMapper

---

## 4. Kiến trúc hệ thống

Hệ thống được xây dựng theo mô hình **Clean Architecture**, gồm các tầng:

- **QLSanBong.API**: Xử lý API backend
- **QLSanBong.Application**: Chứa business logic
- **QLSanBong.Domain**: Định nghĩa entity và interface
- **QLSanBong.Infrastructure**: Xử lý database, repository
- **QLSanBong.MVC**: Giao diện người dùng

---

## 5. Chức năng chính

### Quản lý khách hàng

- Thêm / sửa / xóa khách hàng
- Tìm kiếm khách hàng

### Quản lý sân bóng

- Thêm / sửa / xóa sân
- Quản lý trạng thái sân

### Quản lý đặt sân

- Đặt lịch sân
- Kiểm tra trùng lịch
- Cập nhật trạng thái đặt sân

### Quản lý doanh thu

- Thống kê theo ngày / tháng
- Theo dõi lịch sử giao dịch

---

## 6. Cơ sở dữ liệu

- Sử dụng **SQL Server**
- Thiết kế theo mô hình quan hệ
- Các bảng chính:
  - Customers
  - Fields
  - Bookings
  - Invoices

---

## 7. Hướng dẫn cài đặt & chạy chương trình

### Bước 1: Clone source code

```bash
git clone https://github.com/trandoannhat/ASPNET-DK24TTC5-trandoannhat-QLSanBong.git
cd ASPNET-DK24TTC5-trandoannhat-QLSanBong
```

---

### Bước 2: Cấu hình database

Mở file `appsettings.json` và chỉnh lại chuỗi kết nối:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=QLSanBong;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

---

### Bước 3: Tạo database

Chạy lệnh:

```bash
dotnet ef database update
```

---

### Bước 4: Chạy project

- Mở bằng Visual Studio 2022
- Chọn project startup:
  - `QLSanBong.API` (backend)
  - hoặc `QLSanBong.MVC` (giao diện)

- Nhấn **F5** để chạy

---

## 8. Giao diện hệ thống

_(Có thể bổ sung hình ảnh minh họa tại đây)_

---

## 9. Hướng phát triển

- Tích hợp thanh toán online
- Xây dựng ứng dụng mobile
- Phân quyền người dùng (Admin / Nhân viên)
- Báo cáo nâng cao

---

## 10. Thông tin sinh viên

- Họ tên: **Trần Doãn Nhất**
- Môn học: ASP.NET
- Đề tài: Xây dựng website quản lý sân bóng đá mini

---

## 11. Liên hệ

- Email: doannhatit@gmail.com
- SĐT: 0907011886

---

## 12. Kết luận

Đề tài giúp sinh viên hiểu rõ quy trình xây dựng một hệ thống web thực tế bằng ASP.NET, từ thiết kế kiến trúc đến triển khai và vận hành.

---
