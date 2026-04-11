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
        // SQL Server
        services.AddDbContext<QLSanBongDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Unit of Work & Repositories
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // File Service (Local storage)
        services.AddScoped<IFileService, LocalFileService>();

        return services;
    }
}