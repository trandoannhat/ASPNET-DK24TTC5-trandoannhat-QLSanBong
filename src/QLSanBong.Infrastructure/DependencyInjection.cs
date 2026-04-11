using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QLSanBong.Application.Interfaces;
using QLSanBong.Domain.Interfaces;
using QLSanBong.Infrastructure.Data;
using QLSanBong.Infrastructure.Repositories;
using QLSanBong.Infrastructure.Services;

namespace QLSanBong.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Đăng ký DbContext (SQL Server)
        services.AddDbContext<QLSanBongDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // 2. Đăng ký UnitOfWork (Đã bao hàm tất cả Repositories)
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // 3. Đăng ký các dịch vụ Hạ tầng khác (File, Cloud...)
        services.AddScoped<IFileService, CloudinaryFileService>();

      
        

        return services;
    }
}