using QLSanBong.Application.DTOs.Account;
using QLSanBong.Common.Wrappers;
using QLSanBong.Domain.Enums;

namespace QLSanBong.Application.Interfaces;

public interface IAccountService
{
    Task<ApiResponse<AuthenticationResponse>> AuthenticateAsync(LoginRequest request);
    Task<ApiResponse<string>> RegisterAsync(RegisterRequest request);
    Task<ApiResponse<UserProfileDto>> GetProfileAsync(string userId);
    Task<ApiResponse<string>> UpdateProfileAsync(string userId, UpdateProfileRequest request);
    Task<ApiResponse<string>> ChangePasswordAsync(string userId, ChangePasswordRequest request);

    // Quản lý hệ thống
    Task<ApiResponse<IEnumerable<UserProfileDto>>> GetAllUsersAsync();
    Task<ApiResponse<string>> UpdateUserRoleAsync(string currentUserId, string targetUserId, UserRole newRole);
    Task<ApiResponse<string>> DeleteUserAsync(string currentUserId, string targetUserId);
}