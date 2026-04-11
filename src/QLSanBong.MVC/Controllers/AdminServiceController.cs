using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLSanBong.Application.DTOs.Service;
using QLSanBong.Application.Interfaces;
using QLSanBong.MVC.Models;

namespace QLSanBong.MVC.Controllers;

[Authorize(Roles = "Admin,PitchAdmin")]
public class AdminServiceController(IServiceManagementService serviceManagement) : Controller
{
    // Hiển thị danh sách các mặt hàng (Canteen & Phụ kiện)
    public async Task<IActionResult> Index()
    {
        var response = await serviceManagement.GetAllServicesAsync();
        // Để nhanh gọn, ta truyền thẳng DTO ra View mà không cần ViewModel nếu không có logic phức tạp
        var services = response.Data ?? new List<ServiceDto>();
        return View(services);
    }

    // Xóa mặt hàng
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await serviceManagement.DeleteServiceAsync(id);
        if (result.Success)
            TempData["SuccessMessage"] = "Đã xóa mặt hàng thành công.";
        else
            TempData["ErrorMessage"] = result.Message ?? "Lỗi khi xóa mặt hàng.";

        return RedirectToAction(nameof(Index));
    }
    // GET: Hiển thị form Thêm mới
    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateUpdateServiceViewModel());
    }

    // POST: Xử lý lưu Thêm mới hoặc Cập nhật
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(CreateUpdateServiceViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // Nếu lỗi validate, trả về lại đúng form (Create hoặc Edit)
            return model.Id == Guid.Empty ? View("Create", model) : View("Edit", model);
        }

        var request = new CreateUpdateServiceDto
        {
            Id = model.Id,
            Name = model.Name,
            Price = model.Price,
            Unit = model.Unit,
            Category = model.Category
        };

        var result = await serviceManagement.SaveServiceAsync(request);

        if (result.Success)
        {
            TempData["SuccessMessage"] = model.Id == Guid.Empty ? "Đã thêm mặt hàng mới thành công!" : "Cập nhật mặt hàng thành công!";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, result.Message ?? "Có lỗi xảy ra khi lưu dữ liệu.");
        return model.Id == Guid.Empty ? View("Create", model) : View("Edit", model);
    }

    // GET: Hiển thị form Sửa
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var response = await serviceManagement.GetServiceByIdAsync(id);
        if (!response.Success || response.Data == null)
        {
            TempData["ErrorMessage"] = "Không tìm thấy mặt hàng này.";
            return RedirectToAction(nameof(Index));
        }

        var model = new CreateUpdateServiceViewModel
        {
            Id = response.Data.Id,
            Name = response.Data.Name,
            Price = response.Data.Price,
            Unit = response.Data.Unit,
            Category = response.Data.Category
        };

        return View(model);
    }
}