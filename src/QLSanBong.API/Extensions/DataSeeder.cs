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

        await context.Database.MigrateAsync();
        bool isDataChanged = false;

        // 1. Seed Admin User
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
            Console.WriteLine("-> Seeded Admin User.");
        }

        // 2. Seed Customer User
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
            Console.WriteLine("-> Seeded Customer User.");
        }

        // 3. Seed Pitches
        if (!await context.Pitches.AnyAsync())
        {
            var pitches = new List<Pitch>
            {
                new() { Name = "Sân Mini A1 (5 người)", PitchType = "Sân 5", PricePerHour = 250000, ImageUrl = "https://images.unsplash.com/photo-1529900748604-07564a03e7a6?q=80&w=800" },
                new() { Name = "Sân Mini A2 (5 người)", PitchType = "Sân 5", PricePerHour = 250000, ImageUrl = "https://images.unsplash.com/photo-1574629810360-7efbbe195018?q=80&w=800" },
                new() { Name = "Sân Mini A3 (5 người)", PitchType = "Sân 5", PricePerHour = 250000, ImageUrl = "https://images.unsplash.com/photo-1551958219-acbc608c6377?q=80&w=800" },
                new() { Name = "Sân Mini B1 (7 người)", PitchType = "Sân 7", PricePerHour = 450000, ImageUrl = "https://images.unsplash.com/photo-1579952363873-27f3bade9f55?q=80&w=800" },
                new() { Name = "Sân Mini B2 (7 người)", PitchType = "Sân 7", PricePerHour = 450000, ImageUrl = "https://images.unsplash.com/photo-1431324155629-1a6deb1dec8d?q=80&w=800" },
                new() { Name = "Sân Mini B3 (7 người)", PitchType = "Sân 7", PricePerHour = 450000, ImageUrl = "https://images.unsplash.com/photo-1518005020951-eccb494ad742?q=80&w=800" }
            };

            await context.Pitches.AddRangeAsync(pitches);
            isDataChanged = true;
            Console.WriteLine("-> Seeded Pitches.");
        }

        // 4. Seed Canteen Services
        if (!await context.Services.AnyAsync())
        {
            var services = new List<Service>
            {
                new() { Name = "Nước suối Aquafina", Price = 10000, Unit = "Chai", Category = ServiceCategory.Beverage },
                new() { Name = "Nước tăng lực Sting", Price = 15000, Unit = "Chai", Category = ServiceCategory.Beverage },
                new() { Name = "Nước tăng lực Redbull", Price = 20000, Unit = "Lon", Category = ServiceCategory.Beverage },
                new() { Name = "Thuê áo Bib (Đội)", Price = 50000, Unit = "Bộ", Category = ServiceCategory.Equipment },
                new() { Name = "Thuê bóng", Price = 30000, Unit = "Quả", Category = ServiceCategory.Equipment }
            };
            await context.Services.AddRangeAsync(services);
            isDataChanged = true;
            Console.WriteLine("-> Seeded Services.");
        }

        //  Lưu dữ liệu (User, Sân) xuống Database trước
        // Để Bước 5 bên dưới có thể FindOrDefault ra dữ liệu thực tế.
        if (isDataChanged)
        {
            await context.SaveChangesAsync();
            isDataChanged = false; // Reset lại cờ để chạy tiếp Bước 5
        }

        // 5. Seed Pitch Bookings
        if (!await context.PitchBookings.AnyAsync())
        {
            var customer = await context.Users.FirstOrDefaultAsync(u => u.Email == "khachhang@gmail.com");
            var pitch1 = await context.Pitches.FirstOrDefaultAsync(p => p.PitchType == "Sân 5");
            var pitch2 = await context.Pitches.FirstOrDefaultAsync(p => p.PitchType == "Sân 7");

            if (customer != null && pitch1 != null && pitch2 != null)
            {
                var bookings = new List<PitchBooking>
                {
                    // Lịch sử giao dịch (Đã hoàn thành)
                    new() {
                        UserId = customer.Id,
                        PitchId = pitch1.Id,
                        BookingDate = DateTime.Today.AddDays(-1),
                        StartTime = new TimeSpan(17, 0, 0),
                        EndTime = new TimeSpan(18, 30, 0),
                        TotalPrice = pitch1.PricePerHour * 1.5m,
                        Status = BookingStatus.Approved,
                        Notes = "Khách VIP, đã thanh toán VNPay"
                    },
                    // Lịch đặt sắp tới (Chờ duyệt)
                    new() {
                        UserId = customer.Id,
                        PitchId = pitch2.Id,
                        BookingDate = DateTime.Today.AddDays(1),
                        StartTime = new TimeSpan(19, 0, 0),
                        EndTime = new TimeSpan(20, 30, 0),
                        TotalPrice = pitch2.PricePerHour * 1.5m,
                        Status = BookingStatus.Pending,
                        Notes = "Khách hẹn trả tiền mặt tại sân"
                    }
                };
                await context.PitchBookings.AddRangeAsync(bookings);
                isDataChanged = true;
                Console.WriteLine("-> Seeded Pitch Bookings.");
            }
        }

        // Lưu các Booking vừa được thêm
        if (isDataChanged)
        {
            await context.SaveChangesAsync();
        }
    }
}