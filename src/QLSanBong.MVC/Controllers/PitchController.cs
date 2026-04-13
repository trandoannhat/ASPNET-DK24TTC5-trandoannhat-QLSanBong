using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLSanBong.Application.DTOs.Pitch;
using QLSanBong.Application.Interfaces;
using QLSanBong.Application.Services;
using QLSanBong.Domain.Enums;
using QLSanBong.MVC.Models;
using System.Security.Claims;

namespace QLSanBong.MVC.Controllers;

public class PitchController(IPitchBookingService pitchBookingService, IMapper mapper, VnPayService vnPayService) : Controller
{
    // ==========================================================
    // 1. TÌM KIẾM VÀ XEM SÂN (PUBLIC)
    // ==========================================================

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var apiResponse = await pitchBookingService.GetAllPitchesAsync();
        var pitchViewModels = mapper.Map<IEnumerable<PitchViewModel>>(apiResponse.Data);
        return View(pitchViewModels);
    }

    [HttpGet]
    public async Task<IActionResult> Search(DateTime? date, string type)
    {
        var apiResponse = await pitchBookingService.GetAllPitchesAsync();
        var pitches = apiResponse.Data;

        if (!string.IsNullOrEmpty(type) && pitches != null)
        {
            pitches = pitches.Where(p => p.PitchType == type);
        }

        var pitchViewModels = mapper.Map<IEnumerable<PitchViewModel>>(pitches);

        ViewBag.SearchDate = date?.ToString("yyyy-MM-dd") ?? DateTime.Today.ToString("yyyy-MM-dd");
        ViewBag.SearchType = type;

        return View("Index", pitchViewModels);
    }

    // ==========================================================
    // 2. NGHIỆP VỤ ĐẶT SÂN VÀ THANH TOÁN (PUBLIC / USER)
    // ==========================================================

    [HttpGet]
    public async Task<IActionResult> Book(Guid pitchId)
    {
        var pitchResponse = await pitchBookingService.GetPitchByIdAsync(pitchId);
        if (!pitchResponse.Success || pitchResponse.Data == null)
            return NotFound("Không tìm thấy sân bóng.");

        var model = new BookPitchViewModel
        {
            PitchId = pitchResponse.Data.Id,
            PitchName = pitchResponse.Data.Name,
            PricePerHour = pitchResponse.Data.PricePerHour,
            BookingDate = DateTime.Today
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Book(BookPitchViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var requestDto = new CreateAdminPitchBookingDto
        {
            PitchId = model.PitchId,
            CustomerName = model.CustomerName,
            CustomerPhone = model.CustomerPhone,
            BookingDate = model.BookingDate,
            StartTime = TimeSpan.Parse(model.StartTime),
            EndTime = TimeSpan.Parse(model.EndTime),
            Notes = model.Notes,
            Status = BookingStatus.Pending // Khách đặt luôn ở trạng thái chờ duyệt
        };

        var response = await pitchBookingService.CreateAdminBookingAsync(requestDto);

        if (response.Success)
        {
            // LẤY ID THẬT TỪ DATABASE TRẢ VỀ ĐỂ TRUYỀN SANG VNPAY
            Guid actualBookingId = response.Data;

            if (model.PaymentMethod == "VNPay")
            {
                var duration = (TimeSpan.Parse(model.EndTime) - TimeSpan.Parse(model.StartTime)).TotalHours;
                var depositAmount = (decimal)duration * model.PricePerHour * 0.3m; // Tính tiền cọc 30%

                // Truyền ID thật vào VNPay
                var paymentUrl = vnPayService.CreatePaymentUrl(HttpContext, actualBookingId, depositAmount);
                return Redirect(paymentUrl);
            }

            TempData["SuccessMessage"] = "Yêu cầu đặt sân đã được gửi! Vui lòng chờ bộ phận quản lý xác nhận.";
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, response.Message ?? "Có lỗi xảy ra khi tạo lịch đặt.");
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> PaymentCallback()
    {
        var query = HttpContext.Request.Query;

        if (!vnPayService.ValidateSignature(query))
        {
            TempData["ErrorMessage"] = "Giao dịch không hợp lệ hoặc chữ ký bị sai lệch!";
            return RedirectToAction("Index", "Home");
        }

        var responseCode = query["vnp_ResponseCode"].ToString();
        var bookingIdString = query["vnp_TxnRef"].ToString();

        if (responseCode == "00")
        {
            if (Guid.TryParse(bookingIdString, out Guid bookingId))
            {
                // Xác nhận thanh toán thành công và cập nhật Status
                await pitchBookingService.ConfirmVnPayDepositAsync(bookingId);
            }

            TempData["SuccessMessage"] = "Thanh toán thành công! Lịch đặt sân của bạn đã được xác nhận tự động.";
        }
        else
        {
            TempData["ErrorMessage"] = "Bạn đã hủy thanh toán hoặc giao dịch thất bại.";
        }

        return RedirectToAction("Index", "Home");
    }

    // ==========================================================
    // 3. QUẢN LÝ LỊCH ĐẶT CÁ NHÂN (YÊU CẦU ĐĂNG NHẬP)
    // ==========================================================

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> MyBookings()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var response = await pitchBookingService.GetMyBookingsAsync(userId);
        var viewModels = mapper.Map<IEnumerable<MyBookingViewModel>>(response.Data);

        return View(viewModels);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelBooking(Guid id)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        Guid.TryParse(userIdString, out Guid currentUserId);

        var response = await pitchBookingService.CancelMyBookingAsync(id, currentUserId);

        if (response.Success)
        {
            TempData["SuccessMessage"] = "Đã hủy lịch đặt sân thành công!";
        }
        else
        {
            TempData["ErrorMessage"] = response.Message ?? "Không thể hủy lịch. Lịch này có thể đã được duyệt hoặc bạn không có quyền.";
        }

        return RedirectToAction(nameof(MyBookings));
    }
}