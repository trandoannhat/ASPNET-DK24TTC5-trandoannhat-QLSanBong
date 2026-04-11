using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLSanBong.Application.DTOs.Pitch;
using QLSanBong.Application.Interfaces;
using QLSanBong.Common.Exceptions;

namespace QLSanBong.API.Controllers.Pitch;

[Route("api/pitch/[controller]")]
[ApiController]
public class PitchesController(IPitchBookingService pitchBookingService) : ControllerBase
{
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPitchById(Guid id)
    {
        var result = await pitchBookingService.GetPitchByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    [Authorize(Roles = AppConstants.Roles.PitchManagers)]
    public async Task<IActionResult> CreatePitch([FromBody] CreatePitchDto request)
    {
        var result = await pitchBookingService.CreatePitchAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = AppConstants.Roles.PitchManagers)]
    public async Task<IActionResult> UpdatePitch(Guid id, [FromBody] UpdatePitchDto request)
    {
        if (id != request.Id) return BadRequest(new { Message = "ID sân bóng không khớp." });
        
        var result = await pitchBookingService.UpdatePitchAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = AppConstants.Roles.PitchManagers)]
    public async Task<IActionResult> DeletePitch(Guid id)
    {
        var result = await pitchBookingService.DeletePitchAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}