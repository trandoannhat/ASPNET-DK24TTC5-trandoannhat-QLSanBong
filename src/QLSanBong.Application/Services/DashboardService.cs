using Microsoft.EntityFrameworkCore;
using QLSanBong.Application.DTOs.Dashboard;
using QLSanBong.Application.Interfaces;
using QLSanBong.Domain.Enums;
using QLSanBong.Domain.Interfaces;

namespace QLSanBong.Application.Services;

public class DashboardService(IUnitOfWork unitOfWork) : IDashboardService
{
    public async Task<DashboardDto> GetDashboardDataAsync()
    {
        var today = DateTime.Today;
        var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
        var sevenDaysAgo = today.AddDays(-6);

        // Lấy toàn bộ lịch đặt từ 7 ngày trước đến hết tháng này
        var minDate = firstDayOfMonth < sevenDaysAgo ? firstDayOfMonth : sevenDaysAgo;
        var recentBookings = await unitOfWork.PitchBookings.GetAllQueryable()
            .Where(b => b.BookingDate >= minDate)
            .ToListAsync();

        var dto = new DashboardDto();

        // 1. Tính toán cho 4 thẻ tóm tắt (Chỉ cộng tiền những ca Đã Hoàn Thành)
        var todayBookings = recentBookings.Where(b => b.BookingDate.Date == today).ToList();

        dto.TotalBookingsToday = todayBookings.Count;
        dto.RevenueToday = todayBookings.Where(b => b.Status == BookingStatus.Completed).Sum(b => b.TotalPrice);

        dto.RevenueThisMonth = recentBookings
            .Where(b => b.BookingDate.Month == today.Month && b.Status == BookingStatus.Completed)
            .Sum(b => b.TotalPrice);

        dto.PendingBookings = recentBookings.Count(b => b.Status == BookingStatus.Pending);

        // 2. Chuẩn bị mảng dữ liệu cho Biểu đồ (7 ngày gần nhất)
        for (int i = 0; i < 7; i++)
        {
            var date = sevenDaysAgo.AddDays(i);
            dto.DateLabels.Add(date.ToString("dd/MM")); // Trả về dạng "15/03"

            var dailyRevenue = recentBookings
                .Where(b => b.BookingDate.Date == date && b.Status == BookingStatus.Completed)
                .Sum(b => b.TotalPrice);

            dto.RevenueData.Add(dailyRevenue);
        }

        return dto;
    }
}