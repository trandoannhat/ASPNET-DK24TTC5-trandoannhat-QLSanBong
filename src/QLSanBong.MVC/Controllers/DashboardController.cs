using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLSanBong.Application.Interfaces;
using QLSanBong.Application.Services;

namespace QLSanBong.MVC.Controllers;

[Authorize(Roles = "Admin,PitchAdmin")]
public class DashboardController(IDashboardService dashboardService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var data = await dashboardService.GetDashboardDataAsync();
        return View(data);
    }
}