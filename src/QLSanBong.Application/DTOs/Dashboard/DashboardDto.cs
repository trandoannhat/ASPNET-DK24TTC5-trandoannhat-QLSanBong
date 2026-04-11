namespace QLSanBong.Application.DTOs.Dashboard;

public class DashboardDto
{
    public decimal RevenueToday { get; set; }
    public decimal RevenueThisMonth { get; set; }
    public int TotalBookingsToday { get; set; }
    public int PendingBookings { get; set; }

    public List<string> DateLabels { get; set; } = new();
    public List<decimal> RevenueData { get; set; } = new();
}