using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLSanBong.Application.DTOs.Pitch;
using QLSanBong.Application.Interfaces;
using QLSanBong.Common.Exceptions;
using System.Security.Claims;

namespace QLSanBong.API.Controllers.Pitch;

[Route("api/pitch/[controller]")]
[ApiController]
public class BookingsController(IPitchBookingService pitchBookingService) : ControllerBase
{
    // ==========================================
    // KHU VỰC DÀNH CHO KHÁCH HÀNG
    // ==========================================

    [HttpGet("pitches")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllPitches()
    {
        var result = await pitchBookingService.GetAllPitchesAsync();
        return Ok(result);
    }

    [HttpGet("available-pitches")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAvailablePitches([FromQuery] DateTime date, [FromQuery] string startTime, [FromQuery] string endTime, [FromQuery] string? pitchType)
    {
        var result = await pitchBookingService.GetAvailablePitchesAsync(date, startTime, endTime, pitchType);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("my-bookings")]
    [Authorize]
    public async Task<IActionResult> CreateBooking([FromBody] CreatePitchBookingDto request)
    {
        var userIdString = User.FindFirstValue("id") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid currentUserId))
        {
            return Unauthorized(new { Message = "Vui lòng đăng nhập để đặt sân." });
        }

        var result = await pitchBookingService.CreateBookingAsync(request, currentUserId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("my-bookings")]
    [Authorize]
    public async Task<IActionResult> GetMyBookings()
    {
        var userIdString = User.FindFirstValue("id") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid currentUserId))
        {
            return Unauthorized();
        }

        var result = await pitchBookingService.GetMyBookingsAsync(currentUserId);
        return Ok(result);
    }

    [HttpPut("my-bookings/{id}/cancel")]
    [Authorize]
    public async Task<IActionResult> CancelMyBooking(Guid id)
    {
        var userIdString = User.FindFirstValue("id") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdString, out Guid currentUserId))
            return Unauthorized(new { Message = "Token không hợp lệ." });

        var result = await pitchBookingService.CancelMyBookingAsync(id, currentUserId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("my-bookings/{id}/reschedule")]
    [Authorize]
    public async Task<IActionResult> RescheduleMyBooking(Guid id, [FromQuery] DateTime newDate, [FromQuery] string newStartTime, [FromQuery] string newEndTime)
    {
        var userIdString = User.FindFirstValue("id") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdString, out Guid currentUserId))
            return Unauthorized();

        var result = await pitchBookingService.RescheduleMyBookingAsync(id, currentUserId, newDate, newStartTime, newEndTime);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // ==========================================
    // KHU VỰC DÀNH CHO ADMIN VÀ QUẢN LÝ SÂN
    // ==========================================

    [HttpGet("admin/all")]
    [Authorize(Roles = AppConstants.Roles.PitchManagers)]
    public async Task<IActionResult> GetAllBookings([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
    {
        var result = await pitchBookingService.GetAllBookingsAsync(fromDate, toDate);
        return Ok(result);
    }

    [HttpPut("admin/status")]
    [Authorize(Roles = AppConstants.Roles.PitchManagers)]
    public async Task<IActionResult> UpdateStatus([FromBody] UpdatePitchBookingStatusDto request)
    {
        var result = await pitchBookingService.UpdateBookingStatusAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("admin")]
    [Authorize(Roles = AppConstants.Roles.PitchManagers)]
    public async Task<IActionResult> CreateAdminBooking([FromBody] CreateAdminPitchBookingDto request)
    {
        var result = await pitchBookingService.CreateAdminBookingAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}