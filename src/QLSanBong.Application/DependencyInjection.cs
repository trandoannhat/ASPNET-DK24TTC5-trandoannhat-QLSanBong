using Microsoft.Extensions.DependencyInjection;
using QLSanBong.Application.Interfaces;
using QLSanBong.Application.Services;
using System.Reflection;

namespace QLSanBong.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        // Đăng ký AutoMapper tự động quét các Mapping Profile
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // Đăng ký các dịch vụ cốt lõi
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IDashboardService, DashboardService>();

        // Đăng ký dịch vụ liên quan đến Sân bóng và Đặt sân
        services.AddScoped<IPitchBookingService, PitchBookingService>();

        // Đăng ký dịch vụ quản lý dịch vụ phụ trợ và thanh toán
        services.AddScoped<IServiceManagementService, ServiceManagementService>();
        services.AddScoped<IVnPayService, VnPayService>();

        return services;
    }
}