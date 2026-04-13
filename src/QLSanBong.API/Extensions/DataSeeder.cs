using Microsoft.EntityFrameworkCore;
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

        // Tự động chạy Migration để đảm bảo cấu trúc DB mới nhất
        await context.Database.MigrateAsync();

        bool isDataChanged = false;

        // 1. Tạo tài khoản Admin (Sử dụng Email chuẩn để tránh lỗi Validation)
        var adminEmail = "admin@qlsanbong.com";
        if (!await context.Users.AnyAsync(u => u.Email == adminEmail))
        {
            var adminUser = new User
            {
                FullName = "Quản Trị Viên",
                Email = adminEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123@A"),
                Role = UserRole.Admin,
                CreatedAt = DateTime.UtcNow
            };
            await context.Users.AddAsync(adminUser);
            isDataChanged = true;
            Console.WriteLine("-> Đã tạo tài khoản Admin mẫu thành công!");
        }

        // 2. Tạo tài khoản Khách hàng mẫu
        var clientEmail = "khachhang@gmail.com";
        if (!await context.Users.AnyAsync(u => u.Email == clientEmail))
        {
            var clientUser = new User
            {
                FullName = "Nguyễn Văn Khách",
                Email = clientEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("client123@A"),
                Role = UserRole.Customer,
                CreatedAt = DateTime.UtcNow
            };
            await context.Users.AddAsync(clientUser);
            isDataChanged = true;
            Console.WriteLine("-> Đã tạo tài khoản Khách hàng mẫu thành công!");
        }

        // 3. Tạo dữ liệu Sân bóng Mini (Chỉ tập trung Sân 5 và Sân 7)
        var hasPitches = await context.Pitches.AnyAsync();
        if (!hasPitches)
        {
            var pitches = new List<Pitch>
            {
                // Cụm sân 5 người (Sân Mini tiêu chuẩn)
                new() { Name = "Sân Mini A1 (5 người)", PitchType = "Sân 5", PricePerHour = 250000, ImageUrl = "https://images.unsplash.com/photo-1529900748604-07564a03e7a6?q=80&w=800" },
                new() { Name = "Sân Mini A2 (5 người)", PitchType = "Sân 5", PricePerHour = 250000, ImageUrl = "https://images.unsplash.com/photo-1574629810360-7efbbe195018?q=80&w=800" },
                new() { Name = "Sân Mini A3 (5 người)", PitchType = "Sân 5", PricePerHour = 250000, ImageUrl = "https://images.unsplash.com/photo-1551958219-acbc608c6377?q=80&w=800" },

                // Cụm sân 7 người
                new() { Name = "Sân Mini B1 (7 người)", PitchType = "Sân 7", PricePerHour = 450000, ImageUrl = "https://images.unsplash.com/photo-1579952363873-27f3bade9f55?q=80&w=800" },
                new() { Name = "Sân Mini B2 (7 người)", PitchType = "Sân 7", PricePerHour = 450000, ImageUrl = "https://images.unsplash.com/photo-1431324155629-1a6deb1dec8d?q=80&w=800" },
                new() { Name = "Sân Mini B3 (7 người)", PitchType = "Sân 7", PricePerHour = 450000, ImageUrl = "https://images.unsplash.com/photo-1518005020951-eccb494ad742?q=80&w=800" }
            };

            await context.Pitches.AddRangeAsync(pitches);
            isDataChanged = true;
            Console.WriteLine($"-> Đã tạo {pitches.Count} sân bóng mini mẫu thành công!");
        }

        if (isDataChanged)
        {
            await context.SaveChangesAsync();
        }
    }
}