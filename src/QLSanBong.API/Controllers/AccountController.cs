using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLSanBong.Application.DTOs.Account;
using QLSanBong.Application.Interfaces;
using QLSanBong.Common.Exceptions;
using QLSanBong.Common.Wrappers;
using QLSanBong.Domain.Enums;
using System.Security.Claims;

namespace QLSanBong.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController(IAccountService accountService) : BaseApiController
{
    [HttpPost("authenticate")]
    public async Task<IActionResult> AuthenticateAsync(LoginRequest request)
    {
        var response = await accountService.AuthenticateAsync(request);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(RegisterRequest request)
    {
        var response = await accountService.RegisterAsync(request);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirstValue("id") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(ApiResponse<string>.FailureResponse("Phiên đăng nhập không hợp lệ.", "Unauthorized"));

        var response = await accountService.GetProfileAsync(userId);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPut("profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var userId = User.FindFirstValue("id") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(ApiResponse<string>.FailureResponse("Phiên đăng nhập không hợp lệ.", "Unauthorized"));

        var response = await accountService.UpdateProfileAsync(userId, request);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet("users")]
    [Authorize(Roles = AppConstants.Roles.Admin)]
    public async Task<IActionResult> GetAllUsers()
    {
        var response = await accountService.GetAllUsersAsync();
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPut("users/{id}/role")]
    [Authorize(Roles = AppConstants.Roles.Admin)]
    public async Task<IActionResult> UpdateUserRole(string id, [FromBody] UserRole newRole)
    {
        if (string.IsNullOrEmpty(id)) return Error("ID không hợp lệ", "InvalidId");

        var currentUserId = User.FindFirstValue("id") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId))
            return Unauthorized(ApiResponse<string>.FailureResponse("Từ chối truy cập.", "Unauthorized"));

        var response = await accountService.UpdateUserRoleAsync(currentUserId, id, newRole);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpDelete("users/{id}")]
    [Authorize(Roles = AppConstants.Roles.Admin)]
    public async Task<IActionResult> DeleteUser(string id)
    {
        if (string.IsNullOrEmpty(id)) return Error("ID không hợp lệ", "InvalidId");

        var currentUserId = User.FindFirstValue("id") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId))
            return Unauthorized(ApiResponse<string>.FailureResponse("Từ chối truy cập.", "Unauthorized"));

        var response = await accountService.DeleteUserAsync(currentUserId, id);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = User.FindFirstValue("id") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var response = await accountService.ChangePasswordAsync(userId, request);
        return response.Success ? Ok(response) : BadRequest(response);
    }
}