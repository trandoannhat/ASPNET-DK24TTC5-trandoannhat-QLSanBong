using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using QLSanBong.Application.Interfaces;
using QLSanBong.MVC.Models;
using System.Diagnostics;

namespace QLSanBong.MVC.Controllers;

public class HomeController : Controller
{
    private readonly IPitchBookingService _pitchBookingService;
    private readonly IMapper _mapper;

    public HomeController(IPitchBookingService pitchBookingService, IMapper mapper)
    {
        _pitchBookingService = pitchBookingService;
        _mapper = mapper;
    }
    public IActionResult Index()
    {
        // Kiểm tra xem người dùng đã đăng nhập chưa
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            // Nếu là tài khoản Quản lý -> Đá thẳng vào Dashboard
            if (User.IsInRole("Admin") || User.IsInRole("PitchAdmin"))
            {
                return RedirectToAction("Index", "Dashboard");
            }

            // Nếu là tài khoản Khách hàng (Client) -> Có thể chuyển sang trang Đặt sân của khách
            // return RedirectToAction("Index", "ClientBooking"); 
        }

        // Nếu chưa đăng nhập (Khách vãng lai xem web) thì hiển thị trang Landing Page bình thường
        return View();
    }
    //public async Task<IActionResult> Index()
    //{
    //    var apiResponse = await _pitchBookingService.GetAllPitchesAsync();
    //    var pitchViewModels = _mapper.Map<IEnumerable<PitchViewModel>>(apiResponse.Data);
    //    return View(pitchViewModels);
    //}

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}