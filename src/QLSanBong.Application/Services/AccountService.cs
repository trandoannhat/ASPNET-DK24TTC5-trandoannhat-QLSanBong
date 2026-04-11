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

public class AccountService(IUnitOfWork unitOfWork, IOptions<JwtSettings> jwtSettings) : IAccountService
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    public async Task<ApiResponse<string>> RegisterAsync(RegisterRequest request)
    {
        var existingUser = (await unitOfWork.Users.FindAsync(u => u.Email == request.Email)).FirstOrDefault();
        if (existingUser != null)
            return ApiResponse<string>.FailureResponse("Email này đã được sử dụng.");

        var newUser = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = UserRole.Customer
        };

        await unitOfWork.Users.AddAsync(newUser);
        await unitOfWork.CompleteAsync();

        return ApiResponse<string>.SuccessResponse(newUser.Id.ToString(), "Đăng ký thành công", "Register");
    }

    public async Task<ApiResponse<AuthenticationResponse>> AuthenticateAsync(LoginRequest request)
    {
        var user = (await unitOfWork.Users.FindAsync(u => u.Email == request.Email)).FirstOrDefault();
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return ApiResponse<AuthenticationResponse>.FailureResponse("Thông tin đăng nhập không chính xác.");

        var token = GenerateJwtToken(user);
        var rolesList = user.Role.ToString().Split(',').Select(r => r.Trim()).ToList();

        var response = new AuthenticationResponse
        {
            Id = user.Id.ToString(),
            UserName = user.FullName,
            Email = user.Email,
            AvatarUrl = user.AvatarUrl,
            Roles = rolesList,
            IsVerified = true,
            JWToken = token
        };

        return ApiResponse<AuthenticationResponse>.SuccessResponse(response, "Đăng nhập thành công", "Auth");
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim("id", user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in user.Role.ToString().Split(',').Select(r => r.Trim()))
            claims.Add(new Claim(ClaimTypes.Role, role));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<ApiResponse<UserProfileDto>> GetProfileAsync(string userId)
    {
        var user = (await unitOfWork.Users.FindAsync(u => u.Id.ToString() == userId)).FirstOrDefault();
        if (user == null)
            return ApiResponse<UserProfileDto>.FailureResponse("Không tìm thấy người dùng.");

        var profile = new UserProfileDto
        {
            Id = user.Id.ToString(),
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            AvatarUrl = user.AvatarUrl,
            Roles = user.Role.ToString().Split(',').Select(r => r.Trim()).ToList()
        };

        return ApiResponse<UserProfileDto>.SuccessResponse(profile, "Thành công");
    }

    public async Task<ApiResponse<string>> UpdateProfileAsync(string userId, UpdateProfileRequest request)
    {
        var user = (await unitOfWork.Users.FindAsync(u => u.Id.ToString() == userId)).FirstOrDefault();
        if (user == null)
            return ApiResponse<string>.FailureResponse("Không tìm thấy người dùng.");

        user.FullName = request.FullName;
        user.PhoneNumber = request.PhoneNumber;
        user.AvatarUrl = request.AvatarUrl;

        unitOfWork.Users.Update(user);
        await unitOfWork.CompleteAsync();

        return ApiResponse<string>.SuccessResponse(user.Id.ToString(), "Cập nhật thành công");
    }

    public async Task<ApiResponse<IEnumerable<UserProfileDto>>> GetAllUsersAsync()
    {
        var usersFromDb = await unitOfWork.Users.GetAllQueryable().ToListAsync();
        var users = usersFromDb.Select(u => new UserProfileDto
        {
            Id = u.Id.ToString(),
            FullName = u.FullName,
            Email = u.Email,
            PhoneNumber = u.PhoneNumber,
            AvatarUrl = u.AvatarUrl,
            Roles = u.Role.ToString().Split(',').Select(r => r.Trim()).ToList()
        });

        return ApiResponse<IEnumerable<UserProfileDto>>.SuccessResponse(users, "Lấy danh sách thành công");
    }

    public async Task<ApiResponse<string>> UpdateUserRoleAsync(string currentUserId, string targetUserId, UserRole newRole)
    {
        var user = (await unitOfWork.Users.FindAsync(u => u.Id.ToString() == targetUserId)).FirstOrDefault();
        if (user == null)
            return ApiResponse<string>.FailureResponse("Không tìm thấy người dùng.");

        if (currentUserId == targetUserId && user.Role.HasFlag(UserRole.Admin) && !newRole.HasFlag(UserRole.Admin))
            return ApiResponse<string>.FailureResponse("Bạn không thể tự hạ quyền Admin của chính mình.");

        user.Role = newRole;
        unitOfWork.Users.Update(user);
        await unitOfWork.CompleteAsync();

        return ApiResponse<string>.SuccessResponse(user.Id.ToString(), "Cập nhật quyền thành công");
    }

    public async Task<ApiResponse<string>> DeleteUserAsync(string currentUserId, string targetUserId)
    {
        var user = (await unitOfWork.Users.FindAsync(u => u.Id.ToString() == targetUserId)).FirstOrDefault();
        if (user == null)
            return ApiResponse<string>.FailureResponse("Không tìm thấy người dùng.");

        if (currentUserId == targetUserId)
            return ApiResponse<string>.FailureResponse("Không thể tự xóa chính mình.");

        unitOfWork.Users.Delete(user);
        await unitOfWork.CompleteAsync();

        return ApiResponse<string>.SuccessResponse(targetUserId, "Xóa người dùng thành công");
    }
}