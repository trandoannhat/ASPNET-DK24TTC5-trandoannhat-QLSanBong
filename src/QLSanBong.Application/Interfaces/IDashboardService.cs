using QLSanBong.Application.DTOs.Dashboard;

namespace QLSanBong.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardDataAsync();
    }
}
