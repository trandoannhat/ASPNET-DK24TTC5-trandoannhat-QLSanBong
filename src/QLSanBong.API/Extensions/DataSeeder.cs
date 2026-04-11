﻿﻿﻿﻿﻿using Microsoft.EntityFrameworkCore;
using QLSanBong.Domain.Entities;
using QLSanBong.Domain.Enums;
using QLSanBong.Infrastructure.Data;

namespace QLSanBong.API.Extensions;

public static class DataSeeder
{
    public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<QLSanBongDbContext>();

        // ==========================================
        // BÍ QUYẾT ĐỂ CHẠY MƯỢT TRÊN VPS LÀ ĐÂY:
        // ==========================================
        // Lệnh này sẽ tự động tạo bảng (chạy Update-Database) nếu Database trắng
        await context.Database.MigrateAsync();

        bool isDataChanged = false;

        // 1. Tạo tài khoản Admin
        var hasAdmin = await context.Users.AnyAsync(u => u.Email == "admin");
        if (!hasAdmin)
        {
            var adminUser = new User
            {
                FullName = "System Admin",
                Email = "admin", // Sử dụng trường Email làm tên đăng nhập
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123@A"),
                Role = UserRole.Admin,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await context.Users.AddAsync(adminUser);
            isDataChanged = true;
            Console.WriteLine(" Đã tự động tạo tài khoản Admin thành công!");
        }

        // 2. Tạo tài khoản Client
        var hasClient = await context.Users.AnyAsync(u => u.Email == "client");
        if (!hasClient)
        {
            var clientUser = new User
            {
                FullName = "Normal Client",
                Email = "client", // Sử dụng trường Email làm tên đăng nhập
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("client123@A"),
                Role = UserRole.Client,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await context.Users.AddAsync(clientUser);
            isDataChanged = true;
            Console.WriteLine(" Đã tự động tạo tài khoản Client thành công!");
        }

        // 3. Tạo dữ liệu mẫu cho Sân bóng
        var hasPitches = await context.Pitches.AnyAsync();
        if (!hasPitches)
        {
            var pitches = new List<Pitch>
            {
                new() { Name = "Sân bóng NhatDev - Sân 1 (5 người)", PitchType = "Sân 5", PricePerHour = 250000 },                
                new() { Name = "Sân bóng NhatDev - Sân 2 (5 người)", PitchType = "Sân 5", PricePerHour = 250000 },                
                new() { Name = "Sân bóng NhatDev - Sân 3 (7 người)", PitchType = "Sân 7", PricePerHour = 450000 },
                new() { Name = "Sân bóng NhatDev - Sân 4 (7 người)", PitchType = "Sân 7", PricePerHour = 450000 },
                new() { Name = "Sân bóng NhatDev - Sân 5 (11 người)", PitchType = "Sân 11", PricePerHour = 800000 }
            };

            await context.Pitches.AddRangeAsync(pitches);
            isDataChanged = true;
            Console.WriteLine($" Đã tự động tạo {pitches.Count} sân bóng mẫu thành công!");
        }

        if (isDataChanged)
        {
            await context.SaveChangesAsync();
        }
    }
}