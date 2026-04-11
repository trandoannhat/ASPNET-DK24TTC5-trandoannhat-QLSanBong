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
        var startOfMonth = new DateTime(today.Year, today.Month, 1);
        var weekAgo = today.AddDays(-6);

        var bookings = await unitOfWork.PitchBookings.GetAllQueryable()
            .Where(b => b.BookingDate >= (startOfMonth < weekAgo ? startOfMonth : weekAgo))
            .ToListAsync();

        var dto = new DashboardDto
        {
            TotalBookingsToday = bookings.Count(b => b.BookingDate.Date == today),
            RevenueToday = bookings.Where(b => b.BookingDate.Date == today && b.Status == BookingStatus.Completed).Sum(b => b.TotalPrice),
            RevenueThisMonth = bookings.Where(b => b.BookingDate.Month == today.Month && b.Status == BookingStatus.Completed).Sum(b => b.TotalPrice),
            PendingBookings = bookings.Count(b => b.Status == BookingStatus.Pending)
        };

        for (int i = 0; i < 7; i++)
        {
            var date = weekAgo.AddDays(i);
            dto.DateLabels.Add(date.ToString("dd/MM"));
            dto.RevenueData.Add(bookings.Where(b => b.BookingDate.Date == date && b.Status == BookingStatus.Completed).Sum(b => b.TotalPrice));
        }

        return dto;
    }
}