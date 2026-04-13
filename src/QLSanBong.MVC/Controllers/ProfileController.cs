using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLSanBong.Application.DTOs.Account;
using QLSanBong.Application.Interfaces;
using System.Security.Claims;

namespace QLSanBong.MVC.Controllers;

[Authorize]
public class ProfileController(IAccountService accountService, IFileService fileService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var response = await accountService.GetProfileAsync(userId!);
        return View(response.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateInfo(UpdateProfileRequest request, IFormFile? avatarFile)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Lấy thông tin cũ để giữ lại ảnh nếu người dùng không upload ảnh mới
        var oldProfile = await accountService.GetProfileAsync(userId!);

        if (avatarFile != null)
        {
            // Upload ảnh qua Service và gán URL mới
            request.AvatarUrl = await fileService.UploadImageAsync(avatarFile, "avatars");
        }
        else
        {
            // Giữ nguyên URL ảnh cũ
            request.AvatarUrl = oldProfile.Data?.AvatarUrl;
        }

        var response = await accountService.UpdateProfileAsync(userId!, request);
        if (response.Success)
        {
            TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
            return RedirectToAction(nameof(Index));
        }

        TempData["ErrorMessage"] = response.Message ?? "Có lỗi xảy ra khi cập nhật.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult ChangePassword() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        if (!ModelState.IsValid) return View(request);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var response = await accountService.ChangePasswordAsync(userId!, request);

        if (response.Success)
        {
            TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError("", response.Message ?? "Đổi mật khẩu thất bại.");
        return View(request);
    }
}