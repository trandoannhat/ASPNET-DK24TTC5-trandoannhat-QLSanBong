using Microsoft.Extensions.DependencyInjection;
using QLSanBong.Application.Interfaces;
using QLSanBong.Application.Services;
using System.Reflection;

namespace QLSanBong.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        // 1. Đăng ký AutoMapper tự động quét profile
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // 2. Đăng ký toàn bộ các Business Services
        services.AddScoped<IAccountService, AccountService>();
        
        
        //đăng ký cho sân bóng
        services.AddScoped<IPitchBookingService, PitchBookingService>();

        services.AddScoped<IDashboardService, DashboardService>();


        return services;
    }
}