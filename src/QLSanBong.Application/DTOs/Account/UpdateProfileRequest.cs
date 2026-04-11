using System.ComponentModel.DataAnnotations;

namespace QLSanBong.Application.DTOs.Account;

public class UpdateProfileRequest
{
    [Required(ErrorMessage = "Vui lòng nhập Họ tên")]
    [MaxLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
    public string FullName { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Định dạng số điện thoại không hợp lệ")]
    public string? PhoneNumber { get; set; }

    public string? AvatarUrl { get; set; }
}