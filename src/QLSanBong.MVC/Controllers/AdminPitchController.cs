using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using QLSanBong.Application.DTOs.Pitch;
using QLSanBong.Application.Interfaces;
using QLSanBong.MVC.Models;
using System.IO;

namespace QLSanBong.MVC.Controllers;

[Authorize(Roles = "Admin,PitchAdmin")]
public class AdminPitchController(IPitchBookingService pitchBookingService, IMapper mapper, IWebHostEnvironment webHostEnvironment) : Controller
{
    private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;

    public async Task<IActionResult> Index()
    {
        var apiResponse = await pitchBookingService.GetAllPitchesAsync();
        var pitchViewModels = mapper.Map<IEnumerable<PitchViewModel>>(apiResponse.Data);
        return View(pitchViewModels);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreatePitchViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePitchViewModel model)
    {
        if (ModelState.IsValid)
        {
            var createDto = mapper.Map<CreatePitchDto>(model);

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "pitches");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(fileStream);
                }

                createDto.ImageUrl = "/images/pitches/" + uniqueFileName;
            }
            else
            {
                createDto.ImageUrl = "/images/pitches/default_pitch.jpg";
            }

            // ĐÃ SỬA: Gọi đúng biến pitchBookingService
            var response = await pitchBookingService.CreatePitchAsync(createDto);

            if (response.Success)
            {
                TempData["SuccessMessage"] = "Thêm sân bóng thành công!";
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", response.Message ?? "Lỗi tạo sân");
        }
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Book(Guid pitchId)
    {
        var pitchResponse = await pitchBookingService.GetPitchByIdAsync(pitchId);
        if (!pitchResponse.Success || pitchResponse.Data == null)
        {
            return NotFound("Không tìm thấy sân bóng.");
        }

        var model = new AdminBookPitchViewModel
        {
            PitchId = pitchResponse.Data.Id,
            PitchName = pitchResponse.Data.Name,
            BookingDate = DateTime.Today
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Book(AdminBookPitchViewModel model)
    {
        if (!TimeSpan.TryParse(model.StartTime, out var startTime))
        {
            ModelState.AddModelError("StartTime", "Định dạng giờ bắt đầu không hợp lệ.");
        }

        if (!TimeSpan.TryParse(model.EndTime, out var endTime))
        {
            ModelState.AddModelError("EndTime", "Định dạng giờ kết thúc không hợp lệ.");
        }

        if (ModelState.IsValid && startTime >= endTime)
        {
            ModelState.AddModelError("EndTime", "Giờ kết thúc phải lớn hơn giờ bắt đầu.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var requestDto = new CreateAdminPitchBookingDto
        {
            PitchId = model.PitchId,
            CustomerName = model.CustomerName,
            CustomerPhone = model.CustomerPhone,
            BookingDate = model.BookingDate,
            StartTime = startTime,
            EndTime = endTime,
            Notes = model.Notes,
            Status = (int)model.Status
        };

        var response = await pitchBookingService.CreateAdminBookingAsync(requestDto);

        if (response.Success)
        {
            TempData["SuccessMessage"] = response.Message ?? "Admin đã tạo lịch đặt sân thành công!";
            return RedirectToAction("Index", "AdminBooking");
        }

        ModelState.AddModelError(string.Empty, response.Message ?? "Có lỗi xảy ra khi tạo lịch đặt.");
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var response = await pitchBookingService.GetPitchByIdAsync(id);

        if (!response.Success || response.Data == null)
        {
            TempData["ErrorMessage"] = "Không tìm thấy thông tin sân bóng này.";
            return RedirectToAction(nameof(Index));
        }

        var model = new EditPitchViewModel
        {
            Id = response.Data.Id,
            Name = response.Data.Name,
            PitchType = response.Data.PitchType,
            PricePerHour = response.Data.PricePerHour,
            // Sẵn sàng nhận ImageUrl nếu bạn đã thêm vào DTO
            ImageUrl = response.Data.ImageUrl
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditPitchViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var request = new UpdatePitchDto
        {
            Id = model.Id,
            Name = model.Name,
            PitchType = model.PitchType,
            PricePerHour = model.PricePerHour,
            // ImageUrl sẽ được xử lý giống hàm Create nếu bạn code thêm chức năng đổi ảnh
            ImageUrl = model.ImageUrl
        };

        var result = await pitchBookingService.UpdatePitchAsync(request);

        if (result.Success)
        {
            TempData["SuccessMessage"] = "Đã cập nhật thông tin sân bóng thành công!";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, result.Message ?? "Có lỗi xảy ra khi cập nhật.");
        return View(model);
    }
}