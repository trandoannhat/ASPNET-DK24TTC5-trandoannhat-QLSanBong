namespace QLSanBong.Application.DTOs.Dashboard;

public class DashboardDto
{
    // Bốn thẻ tóm tắt (Cards)
    public decimal RevenueToday { get; set; }
    public decimal RevenueThisMonth { get; set; }
    public int TotalBookingsToday { get; set; }
    public int PendingBookings { get; set; }

    // Dữ liệu mảng để vẽ Biểu đồ (7 ngày gần nhất)
    public List<string> DateLabels { get; set; } = new();
    public List<decimal> RevenueData { get; set; } = new();
}