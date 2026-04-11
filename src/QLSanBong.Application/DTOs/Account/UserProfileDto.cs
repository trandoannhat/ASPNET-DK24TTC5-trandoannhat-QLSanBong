namespace QLSanBong.Application.DTOs.Account;

public class UserProfileDto
{
    public string Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? AvatarUrl { get; set; }
    public List<string> Roles { get; set; } = new List<string>();
    //public string Role { get; set; }
}
