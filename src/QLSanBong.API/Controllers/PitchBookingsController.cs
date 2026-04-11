using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLSanBong.Application.DTOs.Pitch;
using QLSanBong.Application.Interfaces;
using QLSanBong.Common.Exceptions;
using QLSanBong.Common.Wrappers;
using System.Security.Claims;

namespace QLSanBong.API.Controllers.Pitch;

[Route("api/pitch/bookings")]
[ApiController]
public class BookingsController(IPitchBookingService pitchBookingService) : BaseApiController
{
    [HttpGet("pitches")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllPitches()
    {
        var result = await pitchBookingService.GetAllPitchesAsync();
        return result.Success ? Ok(result) : BadRequest(result);
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
            return Unauthorized(ApiResponse<string>.FailureResponse("Vui lòng đăng nhập để đặt sân.", "Unauthorized"));
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
            return Unauthorized(ApiResponse<string>.FailureResponse("Từ chối truy cập.", "Unauthorized"));
        }

        var result = await pitchBookingService.GetMyBookingsAsync(currentUserId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("my-bookings/{id}/cancel")]
    [Authorize]
    public async Task<IActionResult> CancelMyBooking(Guid id)
    {
        var userIdString = User.FindFirstValue("id") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdString, out Guid currentUserId))
            return Unauthorized(ApiResponse<string>.FailureResponse("Token không hợp lệ.", "Unauthorized"));

        var result = await pitchBookingService.CancelMyBookingAsync(id, currentUserId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("my-bookings/{id}/reschedule")]
    [Authorize]
    public async Task<IActionResult> RescheduleMyBooking(Guid id, [FromQuery] DateTime newDate, [FromQuery] string newStartTime, [FromQuery] string newEndTime)
    {
        var userIdString = User.FindFirstValue("id") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdString, out Guid currentUserId))
            return Unauthorized(ApiResponse<string>.FailureResponse("Token không hợp lệ.", "Unauthorized"));

        var result = await pitchBookingService.RescheduleMyBookingAsync(id, currentUserId, newDate, newStartTime, newEndTime);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("admin/all")]
    [Authorize(Roles = AppConstants.Roles.PitchManagers)]
    public async Task<IActionResult> GetAllBookings([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
    {
        var result = await pitchBookingService.GetAllBookingsAsync(fromDate, toDate);
        return result.Success ? Ok(result) : BadRequest(result);
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