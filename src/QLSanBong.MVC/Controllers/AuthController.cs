using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using QLSanBong.Application.DTOs.Account;
using QLSanBong.Application.Interfaces;
using QLSanBong.MVC.Models;

namespace QLSanBong.MVC.Controllers;

public class AuthController(IAccountService accountService) : Controller
{
    // Bổ sung tham số returnUrl
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl; // Giữ lại cho View nếu đăng nhập lỗi

        if (!ModelState.IsValid)
            return View(model);

        var response = await accountService.AuthenticateAsync(new LoginRequest
        {
            Email = model.Email,
            Password = model.Password
        });

        if (response.Success && response.Data != null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, response.Data.Id.ToString()),
                new Claim("id", response.Data.Id.ToString()),
                new Claim(ClaimTypes.Name, response.Data.UserName ?? "User"),
                new Claim(ClaimTypes.Email, response.Data.Email ?? "")
            };

            if (response.Data.Roles != null)
            {
                foreach (var role in response.Data.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties { IsPersistent = model.RememberMe });

            // XỬ LÝ ĐIỀU HƯỚNG

            // 1. Ưu tiên 1: Trả về trang cũ mà User đang xem dở (nếu có và an toàn)
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            // 2. Ưu tiên 2: Điều hướng theo Role
            if (response.Data.Roles != null && (response.Data.Roles.Contains("Admin") || response.Data.Roles.Contains("PitchAdmin")))
            {
                return RedirectToAction("Index", "AdminPitch");
            }

            // 3. Mặc định Khách hàng về Trang chủ
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, response.Message ?? "Đăng nhập thất bại. Vui lòng kiểm tra lại email và mật khẩu.");
        return View(model);
    }

    // Bổ sung tham số returnUrl
    [HttpGet]
    public IActionResult Register(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
            return View(model);

        if (model.Password != model.ConfirmPassword)
        {
            // (Thường thì Validation của DataAnnotations ở ViewModel đã lo việc này rồi, nhưng check thêm cũng tốt)
            ModelState.AddModelError(string.Empty, "Mật khẩu xác nhận không khớp.");
            return View(model);
        }

        var response = await accountService.RegisterAsync(new RegisterRequest
        {
            FullName = model.FullName,
            Email = model.Email,
            Password = model.Password,
            ConfirmPassword = model.ConfirmPassword
        });

        if (response.Success)
        {
            // Đăng ký xong thì chuyển qua trang Login, và nhớ mang theo returnUrl
            return RedirectToAction("Login", new { returnUrl = returnUrl });
        }

        ModelState.AddModelError(string.Empty, response.Message ?? "Đăng ký thất bại. Email có thể đã tồn tại.");
        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}