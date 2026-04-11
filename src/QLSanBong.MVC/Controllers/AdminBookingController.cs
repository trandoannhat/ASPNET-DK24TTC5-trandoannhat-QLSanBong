using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLSanBong.Application.DTOs.Pitch;
using QLSanBong.Application.Interfaces;
using QLSanBong.Domain.Enums;
using QLSanBong.MVC.Models;

namespace QLSanBong.MVC.Controllers;

[Authorize(Roles = "Admin,PitchAdmin")]
public class AdminBookingController(
    IPitchBookingService pitchBookingService,
    IServiceManagementService serviceManagement, // Khai báo Service Canteen
    IMapper mapper) : Controller
{
    // ==========================================
    // 1. QUẢN LÝ LỊCH ĐẶT SÂN CHUNG
    // ==========================================

    [HttpGet]
    public async Task<IActionResult> Index(DateTime? filterDate, string? statusFilter, string? searchString)
    {
        DateTime? fromDate = null;
        DateTime? toDate = null;

        // 1. LOGIC TÌM KIẾM THÔNG MINH
        if (filterDate.HasValue)
        {
            // Kịch bản A: Có chọn ngày rõ ràng -> Chỉ tìm trong ngày đó
            fromDate = filterDate.Value.Date;
            toDate = filterDate.Value.Date;
        }
        else if (!string.IsNullOrEmpty(searchString) || !string.IsNullOrEmpty(statusFilter))
        {
            // Kịch bản B: Cố tình gõ tìm kiếm SĐT hoặc Lọc trạng thái (mà để trống ngày) 
            // -> Xóa giới hạn ngày, lục tìm trên TOÀN BỘ lịch sử
            fromDate = null;
            toDate = null;
        }
        else
        {
            // Kịch bản C: Mới mở trang lên (không search gì) 
            // -> Mặc định load Hôm Nay để tránh bị giật lag vì load quá nhiều data
            fromDate = DateTime.Today;
            toDate = DateTime.Today;
        }

        // 2. Gọi Service lấy dữ liệu theo đúng kịch bản Date ở trên
        var response = await pitchBookingService.GetAllBookingsAsync(fromDate, toDate);
        var bookings = mapper.Map<IEnumerable<BookingViewModel>>(response.Data);

        // 3. Xử lý lọc Trạng thái
        if (!string.IsNullOrEmpty(statusFilter))
        {
            bookings = bookings.Where(b => b.Status != null && b.Status.Contains(statusFilter));
        }

        // 4. Xử lý lục tìm theo Tên / SĐT
        if (!string.IsNullOrEmpty(searchString))
        {
            searchString = searchString.ToLower().Trim();
            bookings = bookings.Where(b =>
                (b.CustomerName != null && b.CustomerName.ToLower().Contains(searchString)) ||
                (b.CustomerPhone != null && b.CustomerPhone.Contains(searchString))
            );
        }

        // 5. Truyền dữ liệu về View để giữ nguyên thông tin trên thanh tìm kiếm
        // Lưu ý: Chỉ truyền ngày nếu người dùng thực sự chọn ngày, nếu không thì để ô Date trống
        ViewBag.FilterDate = filterDate?.ToString("yyyy-MM-dd");
        ViewBag.StatusFilter = statusFilter;
        ViewBag.SearchString = searchString;

        // 6. Lấy danh sách Sân bóng cho Pop-up Đặt Sân Nhanh
        var pitchesResponse = await pitchBookingService.GetAllPitchesAsync();
        ViewBag.Pitches = pitchesResponse.Data ?? new List<Application.DTOs.Pitch.PitchDto>();

        return View(bookings);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateStatus(Guid bookingId, int statusId)
    {
        var request = new UpdatePitchBookingStatusDto
        {
            BookingId = bookingId,
            Status = (BookingStatus)statusId
        };
        await pitchBookingService.UpdateBookingStatusAsync(request);
        TempData["SuccessMessage"] = "Cập nhật trạng thái thành công!";
        return RedirectToAction(nameof(Index));
    }

    // ==========================================
    // 2. ADMIN TẠO LỊCH CHO KHÁCH VÃNG LAI
    // ==========================================

   
    [HttpGet]
    public async Task<IActionResult> Book(Guid pitchId)
    {
        var response = await pitchBookingService.GetPitchByIdAsync(pitchId);
        if (response.Data == null) return NotFound();

        var model = new AdminBookPitchViewModel
        {
            PitchId = pitchId,
            PitchName = response.Data.Name,
            BookingDate = DateTime.Today,

            //  Ép mặc định trạng thái trên Form là "Approved" (Đã Duyệt)
            Status = BookingStatus.Approved
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Book(AdminBookPitchViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var request = new CreateAdminPitchBookingDto
        {
            PitchId = model.PitchId,
            CustomerName = model.CustomerName,
            CustomerPhone = model.CustomerPhone,
            BookingDate = model.BookingDate,
            StartTime = TimeSpan.Parse(model.StartTime),
            EndTime = TimeSpan.Parse(model.EndTime),
            Status = (int)model.Status, // Lưu ý: Ép kiểu nguyên (int) để gửi xuống Service

            Notes = model.Notes
        };

        var result = await pitchBookingService.CreateAdminBookingAsync(request);
        if (!result.Success)
        {
            ModelState.AddModelError("", result.Message);
            return View(model);
        }

        TempData["SuccessMessage"] = "Admin đã đặt lịch thành công!";
        return RedirectToAction(nameof(Index));
    }

    // ==========================================
    // 3. QUẢN LÝ HÓA ĐƠN & DỊCH VỤ (CANTEEN)
    // ==========================================

    // GET: Chi tiết Hóa đơn (Nơi Admin xem tổng tiền và thêm nước)
    [HttpGet]
    public async Task<IActionResult> Invoice(Guid id)
    {
        // 1. Lấy thông tin hóa đơn và các dịch vụ đã gọi
        var response = await pitchBookingService.GetBookingDetailsAsync(id);
        if (!response.Success || response.Data == null)
        {
            TempData["ErrorMessage"] = "Không tìm thấy hóa đơn.";
            return RedirectToAction(nameof(Index));
        }

        // 2. Lấy danh sách toàn bộ mặt hàng Canteen để đổ vào thẻ <select> thêm mới
        var canteenResponse = await serviceManagement.GetAllServicesAsync();
        ViewBag.AvailableServices = canteenResponse.Data ?? new List<Application.DTOs.Service.ServiceDto>();

        // Truyền thẳng DTO sang View cho gọn, không cần tạo ViewModel trung gian
        return View(response.Data);
    }

    // POST: Xử lý cộng tiền dịch vụ vào hóa đơn
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddService(Guid bookingId, Guid serviceId, int quantity)
    {
        var result = await pitchBookingService.AddServiceToBookingAsync(bookingId, serviceId, quantity);

        if (result.Success)
            TempData["SuccessMessage"] = result.Message;
        else
            TempData["ErrorMessage"] = result.Message;

        // Xử lý xong thì quay lại chính trang Hóa đơn đó để Admin xem tiền nhảy lên
        return RedirectToAction(nameof(Invoice), new { id = bookingId });
    }
    // ==========================================
    // TÍNH NĂNG CALENDAR (LỊCH BIỂU TRỰC QUAN)
    // ==========================================

    // 1. Hàm trả về Giao diện trang Lịch
    [HttpGet]
    public IActionResult Calendar()
    {
        return View();
    }

    // 2. API cung cấp dữ liệu JSON cho thư viện FullCalendar vẽ lên màn hình
    [HttpGet]
    public async Task<IActionResult> GetCalendarData(string start, string end)
    {
        // FullCalendar sẽ tự động truyền ngày bắt đầu và kết thúc của tuần/tháng đang xem
        if (!DateTime.TryParse(start, out DateTime startDate) || !DateTime.TryParse(end, out DateTime endDate))
        {
            return Json(new object[] { });
        }

        var response = await pitchBookingService.GetAllBookingsAsync(startDate, endDate);
        if (!response.Success || response.Data == null) return Json(new object[] { });

        // Map dữ liệu từ DB sang chuẩn Event của FullCalendar
        var events = response.Data.Where(b => b.Status != "2" && b.Status != "Cancelled" && b.Status != "Đã hủy") // Ẩn các ca đã hủy cho đỡ rác
            .Select(b => new
            {
                id = b.Id,
                title = $"{b.CustomerName} - {b.PitchName}", // Tiêu đề hiển thị trên ô lịch
                start = $"{b.BookingDate:yyyy-MM-dd}T{b.StartTime}", // Định dạng chuẩn ISO: 2026-03-20T14:00:00
                end = $"{b.BookingDate:yyyy-MM-dd}T{b.EndTime}",
                backgroundColor = GetColorByStatus(b.Status), // Lấy màu theo trạng thái
                borderColor = GetColorByStatus(b.Status),
                textColor = (b.Status == "0" || b.Status == "Pending" || b.Status == "Chờ Xác Nhận") ? "#000" : "#fff", // Chữ đen cho nền vàng, chữ trắng cho nền đậm
                                                                                                                        // Tìm dòng description cũ và thay bằng dòng này:
                description = $"SĐT: {b.CustomerPhone} | Giá: {b.TotalPrice:N0}đ\n" +
                              $"Ghi chú: {(string.IsNullOrEmpty(b.Notes) ? "Không có" : b.Notes)}"
                // description = $"SĐT: {b.CustomerPhone} | Giá: {b.TotalPrice:N0}đ" // Dữ liệu ẩn để lát dùng cho Pop-up
            });

        return Json(events);
    }

    // 3. Hàm phụ trợ chia màu cho các khối lịch
    private string GetColorByStatus(string? status)
    {
        if (status == "0" || status == "Pending" || status == "Chờ Xác Nhận")
            return "#ffc107"; // VÀNG: Chờ duyệt
        if (status == "1" || status == "Approved" || status == "Đã Duyệt (Thành công)")
            return "#198754"; // XANH LÁ: Đã duyệt / Đang đá
        if (status == "3" || status == "Completed" || status == "Đã Hoàn Thành")
            return "#0d6efd"; // XANH DƯƠNG: Đã thanh toán xong
        return "#6c757d"; // XÁM: Mặc định
    }
}