using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using QLSanBong.Application.DTOs.Account;
using QLSanBong.Application.Interfaces;
using QLSanBong.Application.Settings;
using QLSanBong.Common.Wrappers;
using QLSanBong.Domain.Entities;
using QLSanBong.Domain.Enums;
using QLSanBong.Domain.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QLSanBong.Application.Services;

public class AccountService(
    IUnitOfWork unitOfWork,
    IOptions<JwtSettings> jwtSettings) : IAccountService
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    // ==========================================
    // 1. PUBLIC ENDPOINTS (Auth)
    // ==========================================
    public async Task<ApiResponse<string>> RegisterAsync(RegisterRequest request)
    {
        var existingUser = (await unitOfWork.Users.FindAsync(u => u.Email == request.Email)).FirstOrDefault();
        if (existingUser != null)
        {
            return new ApiResponse<string> { Success = false, Message = "Email này đã được sử dụng." };
        }

        var newUser = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = UserRole.Client
        };

        await unitOfWork.Users.AddAsync(newUser);
        await unitOfWork.CompleteAsync();

        return new ApiResponse<string>(data: newUser.Id.ToString(), message: "Đăng ký thành công", "register");
    }

    public async Task<ApiResponse<AuthenticationResponse>> AuthenticateAsync(LoginRequest request)
    {
        var user = (await unitOfWork.Users.FindAsync(u => u.Email == request.Email)).FirstOrDefault();

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return new ApiResponse<AuthenticationResponse>("Thông tin đăng nhập không chính xác.");
        }

        var token = GenerateJwtToken(user);

        // ĐÃ SỬA: Tách chuỗi Enum (vd: "Editor, SpaAdmin") thành mảng ["Editor", "SpaAdmin"]
        var rolesList = user.Role.ToString().Split(',').Select(r => r.Trim()).ToList();

        var response = new AuthenticationResponse
        {
            Id = user.Id.ToString(),
            UserName = user.FullName,
            Email = user.Email,
            AvatarUrl = user.AvatarUrl,
            Roles = rolesList, // Đưa mảng quyền chuẩn xác xuống Frontend
            IsVerified = true,
            JWToken = token
        };

        return new ApiResponse<AuthenticationResponse>(response, "Đăng nhập thành công", "Authenticate");
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim("id", user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // ĐÃ SỬA: Nạp từng quyền riêng lẻ vào Claim để [Authorize] phân tích đúng
        var rolesList = user.Role.ToString().Split(',').Select(r => r.Trim());
        foreach (var role in rolesList)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // ==========================================
    // 2. PROTECTED ENDPOINTS (Self Profile)
    // ==========================================
    public async Task<ApiResponse<UserProfileDto>> GetProfileAsync(string userId)
    {
        var user = (await unitOfWork.Users.FindAsync(u => u.Id.ToString() == userId)).FirstOrDefault();
        if (user == null) return new ApiResponse<UserProfileDto>("Không tìm thấy người dùng.");

        var profile = new UserProfileDto
        {
            Id = user.Id.ToString(),
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            AvatarUrl = user.AvatarUrl,
            Roles = user.Role.ToString().Split(',').Select(r => r.Trim()).ToList()
            //Role = user.Role.ToString()
        };
        return new ApiResponse<UserProfileDto>(profile, "Thành công");
    }

    public async Task<ApiResponse<string>> UpdateProfileAsync(string userId, UpdateProfileRequest request)
    {
        var user = (await unitOfWork.Users.FindAsync(u => u.Id.ToString() == userId)).FirstOrDefault();

        if (user == null)
        {
            return new ApiResponse<string> { Success = false, Message = "Không tìm thấy người dùng." };
        }

        user.FullName = request.FullName;
        user.PhoneNumber = request.PhoneNumber;
        user.AvatarUrl = request.AvatarUrl;

        unitOfWork.Users.Update(user);
        await unitOfWork.CompleteAsync();

        return new ApiResponse<string> { Success = true, Data = user.Id.ToString(), Message = "Cập nhật thành công" };
    }

    // ==========================================
    // 3. ADMIN ENDPOINTS (Quản lý User)
    // ==========================================
    public async Task<ApiResponse<IEnumerable<UserProfileDto>>> GetAllUsersAsync()
    {
        try
        {
            // BƯỚC 1: Lấy dữ liệu từ Database về RAM trước (Ép chạy bằng ToListAsync)
            var usersFromDb = await unitOfWork.Users.GetAllQueryable().ToListAsync();

            // BƯỚC 2: Map dữ liệu trên RAM (Lúc này Split sẽ chạy bằng C# thuần, không bị dịch ra SQL nữa)
            var users = usersFromDb.Select(u => new UserProfileDto
            {
                Id = u.Id.ToString(),
                FullName = u.FullName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                AvatarUrl = u.AvatarUrl,
                // Cách an toàn nhất để Split trong C#:
                Roles = u.Role.ToString().Split(new char[] { ',' }).Select(r => r.Trim()).ToList()
            });

            return new ApiResponse<IEnumerable<UserProfileDto>>(users, "Lấy danh sách người dùng thành công");
        }
        catch (Exception ex)
        {
            return new ApiResponse<IEnumerable<UserProfileDto>>($"Lỗi lấy danh sách: {ex.Message}");
        }
    }

    public async Task<ApiResponse<string>> UpdateUserRoleAsync(string currentUserId, string targetUserId, UserRole newRole)
    {
        try
        {
            var user = (await unitOfWork.Users.FindAsync(u => u.Id.ToString() == targetUserId)).FirstOrDefault();
            if (user == null) return new ApiResponse<string> { Success = false, Message = "Không tìm thấy người dùng." };

            // ĐÃ SỬA: Dùng HasFlag để bắt chính xác quyền Admin thay vì so sánh tuyệt đối ==
            if (currentUserId == targetUserId && user.Role.HasFlag(UserRole.Admin) && !newRole.HasFlag(UserRole.Admin))
            {
                return new ApiResponse<string> { Success = false, Message = "Bạn không thể tự hạ quyền Quản trị viên của chính mình." };
            }

            // ĐÃ SỬA: Cập nhật đếm số Admin an toàn với HasFlag
            if (user.Role.HasFlag(UserRole.Admin) && !newRole.HasFlag(UserRole.Admin))
            {
                var adminCount = await unitOfWork.Users.GetAllQueryable()
                    .CountAsync(u => u.Role.HasFlag(UserRole.Admin));

                if (adminCount <= 1)
                {
                    return new ApiResponse<string> { Success = false, Message = "Hệ thống phải có ít nhất 1 Quản trị viên. Không thể hạ quyền Admin này." };
                }
            }

            user.Role = newRole;
            unitOfWork.Users.Update(user);
            await unitOfWork.CompleteAsync();

            return new ApiResponse<string>(data: user.Id.ToString(), message: "Cập nhật quyền thành công", "UpdateRole");
        }
        catch (Exception ex)
        {
            return new ApiResponse<string> { Success = false, Message = $"Lỗi cập nhật: {ex.Message}" };
        }
    }

    public async Task<ApiResponse<string>> DeleteUserAsync(string currentUserId, string targetUserId)
    {
        try
        {
            var user = (await unitOfWork.Users.FindAsync(u => u.Id.ToString() == targetUserId)).FirstOrDefault();
            if (user == null) return new ApiResponse<string> { Success = false, Message = "Không tìm thấy người dùng." };

            if (currentUserId == targetUserId)
            {
                return new ApiResponse<string> { Success = false, Message = "Bạn không thể tự xóa tài khoản của chính mình." };
            }

            // ĐÃ SỬA: Kiểm tra bảo vệ Admin cuối cùng bằng HasFlag
            if (user.Role.HasFlag(UserRole.Admin))
            {
                var adminCount = await unitOfWork.Users.GetAllQueryable()
                    .CountAsync(u => u.Role.HasFlag(UserRole.Admin));

                if (adminCount <= 1)
                {
                    return new ApiResponse<string> { Success = false, Message = "Hệ thống phải có ít nhất 1 Quản trị viên. Không thể xóa Admin này." };
                }
            }

            unitOfWork.Users.Delete(user);
            await unitOfWork.CompleteAsync();

            return new ApiResponse<string>(data: user.Id.ToString(), message: "Xóa người dùng thành công", "DeleteUser");
        }
        catch (Exception ex)
        {
            return new ApiResponse<string> { Success = false, Message = $"Lỗi khi xóa: {ex.Message}" };
        }
    }
}