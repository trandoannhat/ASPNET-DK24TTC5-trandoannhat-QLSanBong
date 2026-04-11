using Microsoft.AspNetCore.Authentication.Cookies;
using QLSanBong.Application;
using QLSanBong.Application.Interfaces;
using QLSanBong.Application.Services;
using QLSanBong.Application.Settings;
using QLSanBong.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// 1. ĐĂNG KÝ KIẾN TRÚC & SERVICE CHUNG
// ============================================

// Nạp cấu hình JwtSettings để AccountService không bị lỗi
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Gọi các tầng kiến trúc
builder.Services.AddApplicationLayer();
builder.Services.AddInfrastructure(builder.Configuration);

// Đăng ký AutoMapper riêng cho MVC
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Đăng ký Service quản lý (Nếu sau này bạn rảnh, hãy chuyển dòng này vào AddApplicationLayer nhé)
builder.Services.AddScoped<IServiceManagementService, ServiceManagementService>();
builder.Services.AddScoped<VnPayService>();

// Gom chung AddControllersWithViews và Razor Runtime Compilation vào 1 chỗ
var mvcBuilder = builder.Services.AddControllersWithViews();
if (builder.Environment.IsDevelopment())
{
    mvcBuilder.AddRazorRuntimeCompilation();
}

// ============================================
// 2. CONFIG AUTHENTICATION (DÙNG COOKIE CHO MVC)
// ============================================
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Home/Index"; // Cấm truy cập -> Văng ra trang chủ
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(30); // Lưu đăng nhập 30 ngày
    });

var app = builder.Build();

// ============================================
// 3. MIDDLEWARE PIPELINE (THỨ TỰ BẮT BUỘC)
// ============================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Xác thực & Phân quyền (Thứ tự này bắt buộc phải đứng sau Routing và trước Endpoints)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();