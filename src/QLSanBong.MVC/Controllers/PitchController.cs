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

        if (!string.IsNullOrEmpty(type))
        {
            pitches = pitches.Where(p => p.PitchType == type);
        }

        var pitchViewModels = mapper.Map<IEnumerable<PitchViewModel>>(pitches);

        ViewBag.SearchDate = date?.ToString("yyyy-MM-dd") ?? DateTime.Today.ToString("yyyy-MM-dd");
        ViewBag.SearchType = type;

        return View("Index", pitchViewModels);
    }

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
            Status = BookingStatus.Pending // Khách đặt luôn ở trạng thái chờ
        };

        var response = await pitchBookingService.CreateAdminBookingAsync(requestDto);

        if (response.Success)
        {
            // LẤY ID THẬT TỪ SERVICE TRẢ VỀ
            Guid actualBookingId = response.Data;

            if (model.PaymentMethod == "VNPay")
            {
                var duration = (TimeSpan.Parse(model.EndTime) - TimeSpan.Parse(model.StartTime)).TotalHours;
                var depositAmount = (decimal)duration * model.PricePerHour * 0.3m;

                // Truyền ID THẬT vào VNPay
                var paymentUrl = vnPayService.CreatePaymentUrl(HttpContext, actualBookingId, depositAmount);
                return Redirect(paymentUrl);
            }

            TempData["SuccessMessage"] = "Yêu cầu đặt sân đã được gửi!";
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, response.Message);
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
                // Gọi hàm xác nhận thanh toán VNPay (Cập nhật Status = 1 và dán nhãn)
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
    // CÁC HÀM QUẢN LÝ SÂN BÓNG (TẠO MỚI, SỬA, XÓA)
    // ==========================================================

    [HttpGet]
    // [Authorize(Roles = "Admin,PitchAdmin")]
    public IActionResult Create()
    {
        return View(new CreatePitchViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    // [Authorize(Roles = "Admin,PitchAdmin")] 
    public async Task<IActionResult> Create(CreatePitchViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var requestDto = new CreatePitchDto
        {
            Name = model.Name,
            PitchType = model.PitchType,
            PricePerHour = model.PricePerHour,
            ImageUrl = model.ImageUrl
        };

        var response = await pitchBookingService.CreatePitchAsync(requestDto);

        if (response.Success)
        {
            TempData["SuccessMessage"] = "Thêm sân bóng mới thành công!";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, response.Message ?? "Có lỗi xảy ra khi lưu sân bóng.");
        return View(model);
    }

    [HttpGet]
    // [Authorize(Roles = "Admin,PitchAdmin")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var response = await pitchBookingService.GetPitchByIdAsync(id);
        if (!response.Success || response.Data == null)
        {
            return NotFound("Không tìm thấy thông tin sân bóng.");
        }

        var model = new CreatePitchViewModel // Dùng chung view model Create hoặc tạo UpdatePitchViewModel riêng
        {
            Name = response.Data.Name,
            PitchType = response.Data.PitchType,
            PricePerHour = response.Data.PricePerHour,
            ImageUrl = response.Data.ImageUrl
        };

        ViewBag.PitchId = id; // Truyền ID sang view để POST lại
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    // [Authorize(Roles = "Admin,PitchAdmin")]
    public async Task<IActionResult> Edit(Guid id, CreatePitchViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var requestDto = new UpdatePitchDto
        {
            Id = id,
            Name = model.Name,
            PitchType = model.PitchType,
            PricePerHour = model.PricePerHour,
            ImageUrl = model.ImageUrl
        };

        var response = await pitchBookingService.UpdatePitchAsync(requestDto);

        if (response.Success)
        {
            TempData["SuccessMessage"] = "Cập nhật thông tin sân bóng thành công!";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, response.Message ?? "Cập nhật thất bại.");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    // [Authorize(Roles = "Admin,PitchAdmin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await pitchBookingService.DeletePitchAsync(id);
        if (response.Success)
        {
            TempData["SuccessMessage"] = "Đã xóa sân bóng thành công!";
        }
        else
        {
            TempData["ErrorMessage"] = response.Message ?? "Xóa sân bóng thất bại.";
        }
        return RedirectToAction(nameof(Index));
    }


    // ==========================================================
    // CÁC HÀM QUẢN LÝ LỊCH ĐẶT CÁ NHÂN (USER)
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

        // Đã sửa lại đúng tên hàm trong Interface: CancelMyBookingAsync
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
